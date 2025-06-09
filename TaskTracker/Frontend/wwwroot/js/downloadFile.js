window.downloadFile = (filename, base64, mimeType) => {
    const link = document.createElement('a');
    link.download = filename;
    link.href = `data:${mimeType};base64,${base64}`;
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
};
