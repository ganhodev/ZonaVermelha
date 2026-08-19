// =====================
// CONFIGURAÇÃO
// =====================
const API_URL = 'http://localhost:5083';

let latitudeSelecionada = null;
let longitudeSelecionada = null;
let zonasNoMapa = {}; // guarda os círculos do mapa por IdZona

// =====================
// MAPA (Leaflet)
// =====================
const mapa = L.map('mapa').setView([-20.3155, -40.3128], 13); // Vitória-ES

L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    attribution: '© OpenStreetMap contributors'
}).addTo(mapa);

// Clique no mapa seleciona a localização
mapa.on('click', function (e) {
    latitudeSelecionada = e.latlng.lat;
    longitudeSelecionada = e.latlng.lng;
    document.getElementById('texto-coordenadas').textContent =
        `📍 ${latitudeSelecionada.toFixed(5)}, ${longitudeSelecionada.toFixed(5)}`;
});

// =====================
// GEOLOCALIZAÇÃO
// =====================
document.getElementById('btn-usar-localizacao').addEventListener('click', () => {
    if (!navigator.geolocation) {
        alert('Geolocalização não suportada pelo seu navegador.');
        return;
    }

    navigator.geolocation.getCurrentPosition(
        (pos) => {
            latitudeSelecionada = pos.coords.latitude;
            longitudeSelecionada = pos.coords.longitude;
            document.getElementById('texto-coordenadas').textContent =
                `📍 ${latitudeSelecionada.toFixed(5)}, ${longitudeSelecionada.toFixed(5)}`;
            mapa.setView([latitudeSelecionada, longitudeSelecionada], 15);
        },
        () => alert('Não foi possível obter sua localização.')
    );
});

// =====================
// FUNÇÕES DO MAPA
// =====================
function corPorIntensidade(nivel) {
    if (nivel >= 5) return '#ff0000'; // vermelho forte
    if (nivel >= 3) return '#ff6600'; // laranja
    return '#ffaa00';                 // amarelo
}

function adicionarOuAtualizarZona(zona) {
    const cor = corPorIntensidade(zona.nivelIntensidadeZona);

    if (zonasNoMapa[zona.idZona]) {
        // zona já existe no mapa — atualiza a cor
        zonasNoMapa[zona.idZona].setStyle({ color: cor, fillColor: cor });
    } else {
        // zona nova — desenha o círculo
        const circulo = L.circle([zona.latitude, zona.longitude], {
            radius: zona.raioMetros,
            color: cor,
            fillColor: cor,
            fillOpacity: 0.35,
            weight: 2
        }).addTo(mapa);

        circulo.bindPopup(`
            <b>Zona de Risco</b><br>
            Intensidade: ${zona.nivelIntensidadeZona}<br>
            Última atividade: ${new Date(zona.ultimaAtividade).toLocaleString('pt-BR')}
        `);

        zonasNoMapa[zona.idZona] = circulo;
    }
}

function removerZona(idZona) {
    if (zonasNoMapa[idZona]) {
        mapa.removeLayer(zonasNoMapa[idZona]);
        delete zonasNoMapa[idZona];
    }
}

// =====================
// CARREGAR ZONAS INICIAIS
// =====================
async function carregarZonas() {
    const lat = -20.3155;
    const lng = -40.3128;

    try {
        const res = await fetch(`${API_URL}/api/zonas?latitude=${lat}&longitude=${lng}`);
        const zonas = await res.json();
        zonas.forEach(adicionarOuAtualizarZona);
    } catch (e) {
        console.error('Erro ao carregar zonas:', e);
    }
}

// =====================
// SIGNALR
// =====================
const connection = new signalR.HubConnectionBuilder()
    .withUrl(`${API_URL}/hubs/zonas`)
    .withAutomaticReconnect()
    .build();

connection.on('ZonaAtualizada', (zona) => {
    adicionarOuAtualizarZona(zona);
});

connection.on('ZonaExpirada', (idZona) => {
    removerZona(idZona);
});

connection.start()
    .then(() => console.log('SignalR conectado'))
    .catch(e => console.error('Erro SignalR:', e));

// =====================
// ENVIAR RELATO
// =====================
document.getElementById('btn-enviar').addEventListener('click', async () => {
    const usuarioId = document.getElementById('input-usuario').value.trim();
    const descricao = document.getElementById('input-descricao').value.trim();

    if (!usuarioId) {
        alert('Informe seu ID de usuário.');
        return;
    }
    if (!descricao) {
        alert('Descreva a ocorrência.');
        return;
    }
    if (latitudeSelecionada === null || longitudeSelecionada === null) {
        alert('Selecione uma localização no mapa ou use sua localização atual.');
        return;
    }

    try {
        const res = await fetch(`${API_URL}/api/relatos`, {
            method: 'POST',
            headers: { 'Content-Type': 'application/json' },
            body: JSON.stringify({
                descricao,
                latitude: latitudeSelecionada,
                longitude: longitudeSelecionada,
                usuarioId
            })
        });

        if (!res.ok) {
            const erro = await res.json();
            alert('Erro: ' + (erro.erros?.join(', ') || erro.erro || 'Erro desconhecido'));
            return;
        }

        // limpa o formulário
        document.getElementById('input-descricao').value = '';
        latitudeSelecionada = null;
        longitudeSelecionada = null;
        document.getElementById('texto-coordenadas').textContent = 'Nenhuma localização selecionada';

        alert('Relato registrado com sucesso!');
    } catch (e) {
        console.error('Erro ao enviar relato:', e);
        alert('Erro ao conectar com a API.');
    }
});

// =====================
// INICIALIZAÇÃO
// =====================
carregarZonas();