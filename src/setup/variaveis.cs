// Declaração das variáveis principais de todo o projeto, separadas por tipos
byte media_meio = 0,
        direcao_triangulo = 255,
        direcao_saida = 255,
        direcao_entrada = 255,
        lugar = 0;

const byte limite_branco = 55;

const short velocidade_max = 250,
        velocidade_padrao = 230;

short velocidade = 250;

float saida1 = 0,
        saida2 = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0,
        direcao_inicial = 0; // variavel para a posição inical do robô no resgate

int tempo_correcao = 0,
        update_time = 16,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0,
        update_curva = 0,
        timeout = 0,
        init_time = 0;

bool preto0 = false,
        preto1 = false,
        preto2 = false,
        preto3 = false,

        verde0 = false,
        verde1 = false,
        verde2 = false,
        verde3 = false,

        preto_curva_dir = false,
        preto_curva_esq = false,

        pegou_kit = false;

short[] angulos_retos = { 0, 90, 180, 270 };

char lado_ajuste = '0';
