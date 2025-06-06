async function SignIn(id, nombreusuario, rol) {

    const url = "/api/auth/signin";

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify({
                Id: id,
                NombreUsuario: nombreusuario,
                RolId: rol
            })
        });

        if (response.ok) {
            console.log("Call '" + url + "'. Status " + response.status);
            // Procesar la respuesta aquí si es necesario
        } else {
            console.error("Error al llamar a '" + url + "'. Status " + response.status);
        }
    } catch (error) {
        console.error("Error al realizar la solicitud: " + error);
    }
}

async function SignOut() {

    const url = "/api/auth/signout";

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            }
        });

        if (response.status === 200) {
            window.location.href = "/";
            window.location.reload(true);
        }
    } catch (error) {
        console.error("Error occurred:", error);
    }
}

async function SignOutLogin() {

    const url = "/api/auth/signout";

    try {
        const response = await fetch(url, {
            method: "POST",
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            }
        });

        if (response.status === 200) {
            console.log("OK");
        }
    } catch (error) {
        console.error("Error occurred:", error);
    }
}

//Arreglar y terminar 

async function GetClaims() {
    const url = "/api/auth/getclaims";

    try {
        const response = await fetch(url, {
            method: "POST",
            credentials: "include", // Envía las cookies al backend
            headers: {
                "Accept": "application/json",
                "Content-Type": "application/json"
            },
            body: JSON.stringify({})
        });

        if (response.ok) {  // Usa response.ok en lugar de comparar con 200
            return await response.json();
        } else {
            console.error("Error: Server responded with status", response.status);
            return null; // Retorna null si la solicitud falla
        }

    } catch (error) {
        console.error("Error occurred:", error);
        return null; // Retorna null si ocurre una excepción
    }
}