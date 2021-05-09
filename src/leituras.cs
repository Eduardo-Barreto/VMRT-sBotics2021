uint millis() => (uint) bc.Timer();
string cor(int sensor) => bc.ReturnColor(sensor);
int luz(byte sensor) => (int) bc.Lightness(sensor);
int ultra(byte sensor) => (int) bc.Distance(sensor);
float eixo_x() => bc.Compass();
float eixo_y() => bc.Inclination();
float angulo_atuador() => bc.AngleActuator();
float angulo_giro_atuador() => bc.AngleScoop();

void delay(int milissegundos) => bc.Wait(milissegundos);
void som(string nota, int tempo) => bc.PlayNote(1, nota, tempo);
void led(byte R, byte G, byte B) => bc.TurnLedOn(R, G, B);
void print(int linha, object texto) => bc.PrintConsole(linha, texto.ToString());
void limpar_console() => bc.ClearConsole();
void limpar_linha(int linha) => bc.ClearConsoleLine(linha);

bool tem_linha(int sensor) => (bc.returnBlue(sensor) < 33);

bool azul(int sensor){
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 31, media_verde = 40, media_azul = 35;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((vermelho < media_vermelho) && (verde < media_verde) && (azul > media_azul));
}

bool verde(int sensor){
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    print(1, $"{vermelho} | {verde} | {azul}");
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 95));
}

bool preto(int sensor){
    if(sensor == 1 || sensor == 2){
        if(bc.lightness(sensor) < media_meio){
            return true;
        }
    }
    if(sensor == 0 || sensor == 3){
        if(bc.lightness(sensor) < media_fora){
            return true;
        }
    }
    return false;
}

bool branco(int sensor){
    if(sensor == 1 || sensor == 2){
        if(bc.lightness(sensor) > media_meio){
            return true;
        }
    }
    if(sensor == 0 || sensor == 3){
        if(bc.lightness(sensor) > media_fora){
            return true;
        }
    }
    return false;
}

void calibrar(){
    ajustar_linha();
    media_meio = (luz(1) + luz(2)) / 4.2f;
    media_fora = (luz(0) + luz(3)) / 4.2f;

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

	print(3, $"calibragem: {media_meio}");
}

void verifica_calibrar(){
    if(proximo(eixo_x(), saida1)){
        calibrar();
    }

    else if(proximo(eixo_x(), saida2)){
    	calibrar();
    }
}

