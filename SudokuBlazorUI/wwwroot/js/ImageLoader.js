window.getImageDataFromBytes = (fileBytes) => {
    return new Promise((resolve, reject) => {
        try {
            // Create blob from byte array
            const blob = new Blob([new Uint8Array(fileBytes)]);
            const url = URL.createObjectURL(blob);

            const img = new Image();
            img.onload = () => {
                try {
                    const canvas = document.createElement('canvas');
                    const ctx = canvas.getContext('2d');

                    canvas.width = img.width;
                    canvas.height = img.height;

                    ctx.drawImage(img, 0, 0);
                    const imageData = ctx.getImageData(0, 0, img.width, img.height);

                    // Clean up
                    URL.revokeObjectURL(url);

                    resolve({
                        width: img.width,
                        height: img.height,
                        data: Array.from(imageData.data)
                    });
                } catch (error) {
                    URL.revokeObjectURL(url);
                    reject(error);
                }
            };

            img.onerror = () => {
                URL.revokeObjectURL(url);
                reject(new Error('Failed to load image'));
            };

            img.src = url;
        } catch (error) {
            reject(error);
        }
    });
};