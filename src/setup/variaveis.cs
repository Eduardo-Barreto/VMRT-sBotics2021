// Declaração das variáveis principais de todo o projeto, separadas por tipos
float saida1 = 0,
        saida2 = 0,
        media_meio = 0,
        media_fora = 0;

int velocidade_padrao = 185,
        velocidade = 180,
        velocidade_max = 220,
        update_time = 16,
        tempo_correcao = 0,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0;

bool preto0 = false,
        preto1 = false,
        preto2 = false,
        preto3 = false,

        verde0 = false,
        verde1 = false,
        verde2 = false,
        verde3 = false,

        preto_curva_dir = false,
        preto_curva_esq = false;

int[] angulos_retos = { 0, 90, 180, 270 };

string lado_ajuste = "0",
        lugar = "piso";
