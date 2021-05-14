float saida1 = 0,
        saida2 = 0,
        media_meio = 0,
        media_fora = 0;

int velocidade_padrao = 190,
        velocidade = 200,
        velocidade_max = 300,
        update_time = 16,
        tempo_correcao = 0,
        ultima_correcao = 0;

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

int[] angulos_retos = {0, 45, 90, 135, 180, 225, 270};
float map(float val, float minimo, float maximo, float minimoSaida, float maximoSaida)
{
    //"mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo)
{
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo - 1 && atual < objetivo + 1);
}

float converter_graus(float graus)
{
    // converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
    float graus_convertidos = graus;
    graus_convertidos = (graus_convertidos < 0) ? (360 + graus_convertidos) : graus_convertidos;
    graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
    graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
    return graus_convertidos;
}

void levantar_atuador()
{
    bc.actuatorSpeed(150);
    bc.actuatorUp(100);
    if (bc.angleActuator() >= 0 && bc.angleActuator() < 88)
    {
        bc.actuatorSpeed(150);
        bc.actuatorUp(600);
    }
}
int millis() => (int)(bc.Timer());
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
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96));
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

void ler_cor(){
    preto0 = preto(0);
    preto1 = preto(1);
    preto2 = preto(2);
    preto3 = preto(3);

    verde0 = verde(0);
    verde1 = verde(1);
    verde2 = verde(2);
    verde3 = verde(3);

    preto_curva_dir = (tem_linha(0) || cor(0) == "PRETO" || preto(0));
    preto_curva_esq = (tem_linha(3) || cor(3) == "PRETO" || preto(3));
}
void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void rotacionar(int velocidade, int graus) => bc.MoveFrontalAngles(velocidade, graus);
void encoder(int velocidade, int rotacoes) => bc.MoveFrontalRotations(velocidade, rotacoes);
void parar(){bc.MoveFrontal(0, 0);delay(10);}
void travar(){bc.MoveFrontal(0, 0);delay(999999999);}

void girar_esquerda(int graus){
    float objetivo = converter_graus(eixo_x() - graus);

    while(!proximo(eixo_x(), objetivo)){
        mover(-1000, 1000);
    }
    parar();
}

void girar_direita(int graus){
    float objetivo = converter_graus(eixo_x() + graus);

    while(!proximo(eixo_x(), objetivo)){
        mover(1000, -1000);
    }
    parar();
}

void objetivo_esquerda(int objetivo){
    while(!proximo(eixo_x(), objetivo)){
        mover(-1000, 1000);
    }
    parar();
}

void objetivo_direita(int objetivo){
    while(!proximo(eixo_x(), objetivo)){
        mover(1000, -1000);
    }
    parar();
}

void alinhar_angulo(){
    led(255, 255, 0);
    print(2, "Alinhando robô");

    int alinhamento = 0;
    float angulo = eixo_x();

    if((angulo > (359 - 2))
	|| (angulo < (0 + 2))
	|| ((angulo > (90 - 2)) && (angulo < (90 + 2)))
	|| ((angulo > (180 - 2)) && (angulo < (180 + 2)))
	|| ((angulo > (270 - 2)) && (angulo < (270 + 2)))){
		return;
	}

    if((angulo > 315) || (angulo <= 45)){
		alinhamento = 0;
	}
	else if((angulo > 45) && (angulo <= 135)){
		alinhamento = 90;
	}
	else if((angulo > 135) && (angulo <= 225)){
		alinhamento = 180;
	}
	else if((angulo > 225) && (angulo <= 315)){
		alinhamento = 270;
	}

	angulo = eixo_x();

	if((alinhamento == 0) && (angulo > 180)){
		objetivo_direita(alinhamento);
	}else if((alinhamento == 0) && (angulo < 180)){
		objetivo_esquerda(alinhamento);
	}else if(angulo < alinhamento){
		objetivo_direita(alinhamento);
	}else if(angulo > alinhamento){
		objetivo_esquerda(alinhamento);
	}

	limpar_linha(2);
	bc.TurnLedOff();
}

void ajustar_linha(){
    led(255, 255, 0);

	while(cor(0) == "PRETO"){
		bc.onTF(-1000, 1000);
	}
	while(cor(1) == "PRETO"){
		bc.onTF(-1000, 1000);
	}
	while(cor(3) == "PRETO"){
		bc.onTF(1000, -1000);
	}
	while(cor(2) == "PRETO"){
		bc.onTF(1000, -1000);
	}

	parar();
    bc.TurnLedOff();
}
void seguir_linha(){
    print(1, $"Seguindo linha: {velocidade}");
    bc.TurnLedOff();
    ler_cor();

    if((millis() > update_time) && (velocidade < velocidade_max)){
        update_time = millis() + 32;
        velocidade++;
    }

    if(preto1){
        velocidade = velocidade_padrao;
        tempo_correcao = millis() + 210;

        while(tempo_correcao > millis()){
            if(branco(1) || preto(2)){
                break;
            }
            mover(1000, -1000);
        }
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    else if(preto2){
        velocidade = velocidade_padrao;
        tempo_correcao = millis() + 210;

        while(tempo_correcao > millis()){
            if(branco(2) || preto(1)){
                break;
            }
            mover(-1000, 1000);
        }
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    else{
        mover(velocidade, velocidade);
    }
}
bool verifica_verde()
{
    ler_cor();
    if (verde0 || verde1)
    {
        print(1, "CURVA VERDE - Direita");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            led(0, 255, 0);
            som("SOL", 100);
            while (!(tem_linha(3)))
            {
                mover(190, 190);
            }
            som("LÁ", 100);
            while (cor(3) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("SI", 100);
            encoder(300, 9);
            girar_direita(20);
            while(!tem_linha(1)){
                mover(1000, -1000);
                if(Array.IndexOf(angulos_retos, eixo_x()) != -1){
                    break;
                }
            }
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            return true;
        }else{
            return false;
        }
    }

    else if (verde2 || verde3)
    {
        print(1, "CURVA VERDE - Esquerda");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            led(0, 255, 0);
            som("SOL", 100);
            while (!(tem_linha(0)))
            {
                mover(190, 190);
            }
            som("LÁ", 100);
            while (cor(0) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("SI", 100);
            encoder(300, 9);
            girar_esquerda(20);
            while(!tem_linha(2)){
                mover(-1000, 1000);
                if(Array.IndexOf(angulos_retos, eixo_x()) != -1){
                    break;
                }
            }
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            return true;
        }else{
            return false;
        }
    }
    else{
        return false;
    }
}

/*
bool verifica_curva()
{
    ler_cor();

    if (verifica_verde()) { return true; }

    else if (preto_curva_dir)
    {
        // curva direita
    }

    else if (preto_curva_dir)
    {
        // curva direita
    }
}
*/
bool debug = false;

void Main()
{
    calibrar();
    while (!debug)
    {
        verifica_calibrar();
        seguir_linha();
        verifica_verde();
    }

    float menor = 100;
    while (debug)
    {
        ler_cor();
        if (verde2 || verde3)
        {
            print(1, "CURVA VERDE - Esquerda");
            encoder(-300, 2);
            ajustar_linha();
            encoder(300, 2);
            delay(64);
            ler_cor();
            if (verde2 || verde3)
            {
                led(0, 255, 0);
                som("SOL", 100);
                while (!(tem_linha(0)))
                {
                    mover(190, 190);
                }
                som("LÁ", 100);
                while (cor(0) == "PRETO")
                {
                    mover(190, 190);
                }
                parar();
                som("SI", 100);
                encoder(300, 9);
                girar_esquerda(20);
                while(!tem_linha(2)){
                    mover(-1000, 1000);
                    if(Array.IndexOf(angulos_retos, eixo_x()) != -1){
                        break;
                    }
                }
                delay(200);
                ajustar_linha();
                encoder(-300, 2);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
            }
        }
        else
        {
            mover(200, 200);
        }
    }
}
