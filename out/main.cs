int map(int val, int minimo, int maximo, int minimoSaida, int maximoSaida){
    // "mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo){
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo-1 && atual < objetivo+1);
}

float converter_graus(float graus){
    // converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
    float graus_convertidos = graus;
    graus_convertidos = (graus_convertidos < 0) ? (360 + graus_convertidos) : graus_convertidos;
    graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
    graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
    return graus_convertidos;
}
int ultra(int sensor) => (int) bc.Distance(sensor);
int millis() => (int) bc.Timer();
string cor(int sensor) => bc.ReturnColor(sensor);
int luz(int sensor) => (int) bc.Lightness(sensor);
float angulo_atuador() => bc.AngleActuator();
float angulo_giro_atuador() => bc.AngleScoop();
float eixo_x() => bc.Compass();
float eixo_y() => bc.Inclination();

void delay(int milissegundos) => bc.Wait(milissegundos);
void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void parar(){bc.MoveFrontal(0, 0);delay(10);}
void travar(){bc.MoveFrontal(0, 0);delay(999999);}

bool tem_linha(int sensor) => (bc.returnBlue(sensor) < 33);


void Main(){
    bc.PrintConsole(1, millis().ToString());
}
