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

