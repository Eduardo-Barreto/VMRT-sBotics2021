int[] possibilidades = new int[360] { 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75, 76, 76, 76, 76, 77, 77, 77, 77, 78, 78, 79, 79, 80, 80, 81, 81, 82, 83, 84, 84, 85, 86, 86, 87, 88, 89, 91, 92, 93, 94, 95, 97, 99, 101, 102, 102, 103, 103, 103, 103, 103, 103, 103, 103, 104, 104, 104, 105, 105, 105, 106, 106, 107, 107, 108, 109, 109, 110, 111, 112, 113, 114, 114, 115, 117, 118, 119, 120, 122, 124, 124, 126, 128, 130, 131, 133, 135, 137, 140, 142, 145, 147, 150, 153, 156, 160, 163, 167, 171, 175, 182, 185, 190, 195, 201, 211, 215, 222, 231, 244, 249, 260, 272, 284, 299, 311, 309, 307, 306, 304, 303, 302, 301, 300, 299, 298, 297, 297, 296, 296, 295, 295, 295, 295, 295, 295, 295, 295, 296, 296, 296, 297, 298, 299, 299, 301, 301, 303, 304, 305, 308, 308, 310, 312, 315, 316, 318, 320, 323, 326, 328, 331, 334, 337, 343, 344, 348, 352, 342, 338, 330, 322, 311, 308, 302, 295, 287, 284, 279, 274, 267, 265, 261, 257, 253, 248, 246, 243, 240, 236, 234, 232, 229, 226, 225, 222, 220, 218, 217, 215, 213, 211, 210, 209, 208, 206, 205, 204, 203, 202, 202, 201, 200, 199, 199, 198, 198, 197, 197, 197, 197, 196, 196, 196, 196, 196, 197, 197, 197, 197, 197, 198, 198, 199, 199, 200, 201, 201, 202, 203, 204, 205, 206, 207, 209, 210, 212, 213, 215, 216, 218, 218, 207, 201, 198, 192, 187, 179, 177, 173, 169, 165, 161, 158, 155, 152, 147, 140, 141, 141, 139, 137, 135, 132, 131, 129, 127, 126, 124, 123, 122, 120, 119, 118, 117, 116, 115, 114, 113, 112, 111, 111, 110, 109, 109, 108, 108, 107, 107, 106, 106, 106, 105, 105, 105, 105, 105, 104, 104, 104, 104, 104, 103, 100, 99, 97, 96, 95, 94, 92, 91, 90, 89, 87, 87, 86, 85, 84, 83, 83, 82, 81, 81, 80, 80, 79, 79, 78, 78, 77, 77, 77, 76, 76, 76, 76, 76, 75, 75, 75, 75, 75, 75, 75, 75, 75, 75 };

//poss 0 = ultra de baixo 
//poss 1 = ultra de cima
//poss 2 = grau
float[,] distancia_grau = new float[360, 3];

//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = angulo
float[,] xy_cru = new float[360, 5];

//poss 0 = x_baixo 
//poss 1 = y_baixo
//poss 2 = x_alto
//poss 3 = y_alto
//poss 4 = objeto_baixo
//poss 5 = objeto_alto
float[,] xy_zerado = new float[360, 6];

//poss 0 = x
//poss 1 = y
//poss 2 = x
//poss 3 = y
float[] xy_robo = new float[2];
float[] xy_entrada = new float[2];
float[] xy_saida = new float[2];
float[] xy_parede = new float[4];
float[] xy_triangulo = new float[2];

float menor_valor = 0.0f,
      maior_valor = 0.0f;

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
    tag_entrada = 0,
    tag_vitima1 = 0,
    tag_vitima2 = 0,
    tag_vitima3 = 0,
    vitima2 = 0,
    vitima3 = 0,
    vitima1 = 0,
    proximidade_vitima = 12;

float direcao_x;
float direcao_y;
float angulo_objetivo;
float distancia_mover_xy;