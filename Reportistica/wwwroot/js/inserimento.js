document.addEventListener('DOMContentLoaded', () => {
    const tipoSelect = document.getElementById('tipoShow');
    const showSelect = document.getElementById('show');
    const episodioSelect = document.getElementById('episodio');

    tipoSelect.addEventListener('change', async () => {
        const tipo = tipoSelect.value;

        showSelect.innerHTML = '<option value="">-- seleziona --</option>';
        showSelect.disabled = true;
        episodioSelect.innerHTML = '<option value="">-- seleziona --</option>';
        episodioSelect.disabled = true;

        if (!tipo) return;

        try {
            const res = await fetch('/api/Show');
            const shows = await res.json();

            const filteredShows = shows.filter(s => s.tipo === tipo);

            filteredShows.forEach(s => {
                const opt = document.createElement('option');
                opt.value = s.id;
                opt.text = s.titolo;
                showSelect.appendChild(opt);
            });

            showSelect.disabled = filteredShows.length === 0;
        } catch {
            console.error("Errore fetch Show:", err);
        }
    });

    showSelect.addEventListener('change', async () => {
        const showId = showSelect.value;

        episodioSelect.innerHTML = '<option value="">-- seleziona --</option>';
        episodioSelect.disabled = true;

        if (!showId) return;

        try {
            const res = await fetch('/api/Episodio');
            const episodi = await res.json();
            debugger;
            const filtered = episodi.filter(e => e.showId === parseInt(showId));

            filtered.forEach(e => {
                const opt = document.createElement('option');
                opt.value = e.id;
                opt.text = e.nome;
                episodioSelect.appendChild(opt);
            });

            if (filtered.length === 1) {
                episodioSelect.disabled = false;
                episodioSelect.value = filtered[0].id;
            } else {
                episodioSelect.disabled = filtered.length === 0;
            }
        } catch {
            console.error("Errore fetch Episodio:", err);
        }
    });
});
