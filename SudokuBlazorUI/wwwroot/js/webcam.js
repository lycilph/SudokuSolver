//function startVideo(src) {
//    if (navigator.mediaDevices && navigator.mediaDevices.getUserMedia) {
//        navigator.mediaDevices.getUserMedia({ video: true }).then(function (stream) {
//            let video = document.getElementById(src);
//            if ("srcObject" in video) {
//                video.srcObject = stream;
//            } else {
//                video.src = window.URL.createObjectURL(stream);
//            }
//            video.onloadedmetadata = function (e) {
//                video.play();
//            };
//        });
//    }
//}

async function startVideo(src) {
    if (!navigator.mediaDevices || !navigator.mediaDevices.getUserMedia) {
        throw new Error("getUserMedia is not supported in this browser");
    }

    try {
        const stream = await navigator.mediaDevices.getUserMedia({ video: true });
        const video = document.getElementById(src);

        if ("srcObject" in video) {
            video.srcObject = stream;
        } else {
            // Fallback for older browsers
            video.src = window.URL.createObjectURL(stream);
        }

        // Return a promise that resolves when the video is playing
        return new Promise((resolve, reject) => {
            video.onloadedmetadata = () => {
                video.play().then(() => {
                    resolve();
                }).catch((error) => {
                    reject(new Error("Failed to play video: " + error.message));
                });
            };
            video.onerror = () => {
                reject(new Error("Video element error"));
            };
        });
    } catch (error) {
        throw new Error("Failed to access webcam: " + error.message);
    }
}

function getFrame(src, dest, dotNetHelper) {
    let video = document.getElementById(src);
    let canvas = document.getElementById(dest);
    canvas.getContext('2d').drawImage(video, 0, 0, 320, 240);

    let dataUrl = canvas.toDataURL("image/jpeg");
    dotNetHelper.invokeMethodAsync('ProcessImage', dataUrl);
}