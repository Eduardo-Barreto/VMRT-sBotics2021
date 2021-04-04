float   saida1 = 0,
        saida2 = 0,
		mediaCurva = 40,
		mediaMeio = 0,
        mediaFora = 0;

// Funções do sbotics

// Com retorno
Func<int, int> ultra = (sensor) => (int)bc.distance(sensor);

Func<uint> millis = () => (uint)bc.millis();

Func<int,string> cor = (sensor) => bc.returnColor(sensor);

Func<int,float> luz = (sensor) => bc.lightness(sensor);

Func <byte, bool> tem_linha = (sensor) => (bc.returnBlue(sensor) < 33);

Func<bool> tem_vitima = () => bc.hasVictims();

Func<float> angulo_atuador = () => bc.angleActuator();

Func<float> angulo_espatula = () => bc.angleBucket();

Func<float> velocidade_atual = () => bc.motorSpeed();

Func<float> eixo_x = () => bc.compass();

Func<float> eixo_y = () => bc.inclination();

// Sem retorno

Action<int> delay = (milissegundos) => bc.wait(milissegundos);

Action<float, float> mover = (vel_esquerda, vel_direita) => bc.onTF(vel_direita, vel_esquerda);

Action parar = () => {bc.onTF(0, 0);delay(10);};

Action travar = () => {bc.onTF(0, 0);delay(999999);};

Action<float, float> rotacionar = (forca, graus) => bc.onTFRot(forca, graus);

Action<string, int> som = (nota, tempo) => bc.playNote(1, nota, tempo);

Action<byte, byte, byte> led = (ledR, ledG, ledB) => bc.turnLedOn(ledR, ledG, ledB);

Action<float, float> encoder = (vel, rotacoes) => bc.onTFRotations(vel, rotacoes);

Action<int, object> print = (linha, texto) => bc.printLCD(linha, texto.ToString());

Action limpar_console = () => bc.clearLCD();

Action<int> vel_atuador = (vel) => bc.actuatorSpeed(vel);

// Funções com retorno (Func) para ajuste

Func <byte, bool> tem_azul = (sensor) => {
   float valVermelho = bc.returnRed(sensor);
   float valVerde = bc.returnGreen(sensor);
   float valAzul = bc.returnBlue(sensor);
   byte mediaVermelho = 31, mediaVerde = 40, mediaAzul = 35;
   int RGB = (int)(valVermelho + valVerde + valAzul);
   sbyte vermelho = (sbyte)(map(valVermelho, 0, RGB, 0, 100));
   sbyte verde = (sbyte)(map(valVerde, 0, RGB, 0, 100));
   sbyte azul = (sbyte)(map(valAzul, 0, RGB, 0, 100));
   return ((vermelho < mediaVermelho) && (verde < mediaVerde) && (azul > mediaAzul));
};

Action<int> girar_esquerda = (graus) => {
	float objetivo = converter_graus(eixo_x() - graus);

	while(!intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			mover(-1000, 1000);
	}
	parar();
};

Action<int> girar_direita = (graus) => {
	float objetivo = converter_graus(eixo_x() + graus);

	while(!intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			mover(1000, -1000);
	}
	parar();
};


Action<int> objetivo_esquerda = (objetivo) => {
	while(!intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			mover(-1000, 1000);
	}
	parar();
};

Action<int> objetivo_direita = (objetivo) => {
	while(!intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			mover(1000, -1000);
	}
	parar();
};

Action alinhar_angulo = () => {
	led(255, 255, 0);
	print(2, "Alinhando robô");

	int alinhamento = 0;
	float gyro = eixo_x();

	if((gyro > (359 - 2))
	|| (gyro < (0 + 2))
	|| ((gyro > (90 - 2)) && (gyro < (90 + 2)))
	|| ((gyro > (180 - 2)) && (gyro < (180 + 2)))
	|| ((gyro > (270 - 2)) && (gyro < (270 + 2)))){
		return;
	}

	if((gyro > 315) || (gyro <= 45)){
		alinhamento = 0;
	}
	else if((gyro > 45) && (gyro <= 135)){
		alinhamento = 90;
	}
	else if((gyro > 135) && (gyro <= 225)){
		alinhamento = 180;
	}
	else if((gyro > 225) && (gyro <= 315)){
		alinhamento = 270;
	}

	gyro = eixo_x();

	if((alinhamento == 0) && (gyro > 180)){
		objetivo_direita(alinhamento);
	}else if((alinhamento == 0) && (gyro < 180)){
		objetivo_esquerda(alinhamento);
	}else if(gyro < alinhamento){
		objetivo_direita(alinhamento);
	}else if(gyro > alinhamento){
		objetivo_esquerda(alinhamento);
	}

	print(2, "");
	led(0, 0, 0);
};

Func <int, bool> preto = (sensor) => {
    if(sensor == 1 || sensor == 2){
        if(bc.lightness(sensor) < mediaMeio){
            return true;
        }
    }
    if(sensor == 0 || sensor == 3){
        if(bc.lightness(sensor) < mediaFora){
            return true;
        }
    }
    return false;
};

Func <int, bool> branco = (sensor) => {
    if(sensor == 1 || sensor == 2){
        if(bc.lightness(sensor) > mediaMeio){
            return true;
        }
    }
    if(sensor == 0 || sensor == 3){
        if(bc.lightness(sensor) > mediaCurva){
            return true;
        }
    }
    return false;
};

Action ajustar_linha = () => {
    bc.turnLedOn(255, 255, 0);

	while(preto(0)){
		bc.onTF(-1000, 1000);
	}
	while(preto(1)){
		bc.onTF(-1000, 1000);
	}
	while(preto(3)){
		bc.onTF(1000, -1000);
	}
	while(preto(2)){
		bc.onTF(1000, -1000);
	}

	parar();
    bc.turnLedOff();
};

Action calibrar = () =>{
    ajustar_linha();
    mediaMeio = (luz(1) + luz(2)) / 4.2f;
    mediaFora = (luz(0) + luz(3)) / 4.2f;

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

	print(3, $"calibragem: {mediaMeio}");
};

Action verifica_calibrar = () =>{
	if(intervalo(eixo_x(), saida1 - 25, saida1 + 25)){
        calibrar();
    }

    else if(intervalo(eixo_x(), saida2 - 25, saida2 + 25)){
    	calibrar();
    }
};
