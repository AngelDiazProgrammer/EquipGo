function startScanner (dotNetHelper) {
    console.log("🚀 startScanner() ejecutándose");

    const videoContainerId = 'scanner-video';
    const videoElement = document.getElementById(videoContainerId);

    if (!videoElement) {
        console.error(`❌ El contenedor #${videoContainerId} no se encontró.`);
        return;
    }

    if (Quagga.initialized) {
        Quagga.stop(); // Para evitar múltiples instancias
    }

    Quagga.init({
        inputStream: {
            type: "LiveStream",
            target: videoElement,
            constraints: {
                facingMode: "environment" // Usa la cámara trasera si está disponible
            }
        },
        decoder: {
            readers: ["code_128_reader", "ean_reader", "ean_8_reader", "code_39_reader"]
        },
        locate: true
    }, function (err) {
        if (err) {
            console.error(err);
            alert("Error al iniciar el escáner.");
            return;
        }

        Quagga.initialized = true;
        Quagga.start();
        console.log("✅ QuaggaJS iniciado correctamente.");

        Quagga.onDetected(function (data) {
            if (data && data.codeResult && data.codeResult.code) {
                const scannedCode = data.codeResult.code;
                console.log("📦 Código detectado:", scannedCode);

                Quagga.stop();

                // Llama a Blazor con logs para detectar errores
                dotNetHelper.invokeMethodAsync('ProcesarCodigo', scannedCode)
                    .then(() => {
                        console.log("✅ ProcesarCodigo invocado exitosamente.");
                    })
                    .catch(err => {
                        console.error("❌ Error al invocar ProcesarCodigo:", err);
                    });
            }
        });
    });
};
