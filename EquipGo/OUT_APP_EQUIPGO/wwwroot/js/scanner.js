function startScanner(dotnetHelper) {
    console.log("🚀 startScanner() ejecutándose");

    const videoElement = document.querySelector('#video');
    if (!videoElement) {
        console.error("❌ El contenedor #video no se encontró.");
        return;
    }
    console.log("✅ Contenedor #video encontrado");

    Quagga.init({
        inputStream: {
            name: "Live",
            type: "LiveStream",
            target: videoElement
        },
        decoder: {
            readers: ["code_128_reader", "ean_reader", "ean_8_reader"]
        }
    }, function (err) {
        if (err) {
            console.error("❌ Error en Quagga.init:", err);
            return;
        }
        console.log("✅ Quagga iniciado correctamente");
        Quagga.start();
    });

    Quagga.onProcessed(function (result) {
        console.log("🎥 Quagga procesando frame...");
    });

    Quagga.onDetected(function (result) {
        console.log("🔎 Código detectado:", result.codeResult.code);
        Quagga.stop();
        dotnetHelper.invokeMethodAsync('ProcesarCodigo', result.codeResult.code);
    });
}

// 👇 Esto asegura que la función esté disponible globalmente
window.startScanner = startScanner;
