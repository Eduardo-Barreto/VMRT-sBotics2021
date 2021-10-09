// Variaveis utilizadas no resgate

float[,] distancia_grau = new float[360, 3];
//poss 0 = ultra de baixo 
//poss 1 = ultra de cima
//poss 2 = grau

float[,] xy_cru = new float[360, 5];
//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = angulo

float[,] xy_zerado = new float[360, 6];
//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = objeto_baixo
//poss 5 = objeto_alto

float[] xy_robo = new float[2];
float[] xy_entrada = new float[2];
float[] xy_saida = new float[2];
float[] xy_parede = new float[4];
float[] xy_triangulo = new float[2];
float[] xy_resgate = new float[2];
//poss 0 = x
//poss 1 = y
//poss 2 = x
//poss 3 = y

// referencias para xy_
const byte x_baixo = 0,
           y_baixo = 1,
           x_alto = 2,
           y_alto = 3,
           angulo_xy = 4,

// referencias para xy_zerado e seus objetos    
           objeto_baixo = 4,
           objeto_alto = 5,
           parede = 1,
           triangulo = 2,
           saida = 4,

// referencias para distancia_grau
           medida_baixa = 0,
           medida_alto = 1,
           angulo_leitura = 2,

//medidas do robô
           raio_l = 18,
           raio_c = 28;

int qualidade_x = 0,
    qualidade_y = 0,
    parede_400 = 0,
    parede_300 = 0,
    inicio_saida = 0,
    termino_saida = 0,
    inicio_saida2 = 0,
    termino_saida2 = 0,
    tag_entrada = 0;

// variaveis para mover_xy e mover_xy_costas
float direcao_x,
      direcao_y,
      angulo_objetivo,
      distancia_mover_xy,
// variavel para as varreduras
      menor_valor = 0;