window.authInterop = {

    login: async function (loginModel) {
        const response = await fetch('/api/auth/login', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(loginModel),
            credentials: 'same-origin'
        });

        if (!response.ok) {
            let errorText;
            try {
                errorText = await response.json();
            } catch {
                errorText = { mensaje: 'Error desconocido' };
            }
            throw new Error(errorText.mensaje || 'Error al iniciar sesión.');
        }

        return await response.json();
    },

    logout: async function () {
        const response = await fetch('/api/auth/logout', {
            method: 'POST',
            credentials: 'same-origin'
        });

        if (!response.ok) {
            let errorText;
            try {
                errorText = await response.json();
            } catch {
                errorText = { mensaje: 'Error desconocido' };
            }
            throw new Error(errorText.mensaje || 'Error al cerrar sesión.');
        }
    }
};
