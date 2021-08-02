// Declaração das variáveis principais de todo o projeto, separadas por tipos
byte velocidade = 180,
        media_meio = 0,
        direcao_triangulo = 0,
        direcao_saida = 0,
        lugar = 0;

const byte velocidade_padrao = 185,
        velocidade_max = 220,
        limite_branco = 55;

float saida1 = 0,
        saida2 = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0;

int tempo_correcao = 0,
        update_time = 16,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0,
        update_curva = 0;

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

short[] angulos_retos = { 0, 90, 180, 270 };

char lado_ajuste = '0';
