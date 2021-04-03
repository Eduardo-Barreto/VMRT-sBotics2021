/*Last change: 03/04/2021 | 00:38:44
----------------------------------------------------------------------------------------------------*/

// Funções de cálculo

Func<float, float, float, float> constrain = delegate(float amt,float low,float high){
   return ((amt)<(low)?(low):((amt)>(high)?(high):(amt)));
};

Func<float, float, float, float, float, int> map = delegate(float value, float min, float max, float minTo, float maxTo) {
  return (int)( (((value - min) * (maxTo - minTo)) / (max - min)) + minTo);
};

Func<float, float> converter_graus= (graus) => {
	float graus_convertidos = graus;
	graus_convertidos = (graus_convertidos < 0) ? 360 + graus_convertidos : graus_convertidos;
	graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
	graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
	return graus_convertidos;
};

Func <float, float, float, bool> intervalo = (val, minimo, maximo) => (val > minimo && val < maximo);

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
   sbyte R = (sbyte)(constrain(map(valVermelho, 0, RGB, 0, 100), 0, 100));
   sbyte G = (sbyte)(constrain(map(valVerde, 0, RGB, 0, 100), 0, 100));
   sbyte B = (sbyte)(constrain(map(valAzul, 0, RGB, 0, 100), 0, 100));
   return ((R < mediaVermelho) && (G < mediaVerde) && (B > mediaAzul));
};

Action<int> girar_esquerda = (graus) => {
	float objetivo = converter_graus(eixo_x() - graus);

	while(true){
		if(intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			break;
		}
      	else{
			mover(-1000, 1000);
		}
	}
	parar();
};

Action<int> girar_direita = (graus) => {
	float objetivo = converter_graus(eixo_x() + graus);

	while(true){
		if(intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			break;
		}
    	else{
			mover(1000, -1000);
		}
	}
	parar();
};


Action<int> objetivo_esquerda = (objetivo) => {
	while(true){
		if(intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			break;
		}
		else{
			mover(-1000, 1000);
		}
	}
	parar();
};

Action<int> objetivo_direita = (objetivo) => {
	while(true){
		if(intervalo(eixo_x(), (objetivo - 1), (objetivo + 1))){
			break;
		}
		else{
			mover(1000, -1000);
		}
	}
	parar();
};

Action alinhar_angulo = () => {
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

	print(2, $"ALINHANDO");
	print(3, $"de {gyro} para {alinhamento}");

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
	limpar_console();
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


float   velocidade_padrao = 190,
        velocidade = 200,
        velocidade_max = 300,
        update_time = millis() + 16;

uint    tempo_correcao = 0,
        ultima_correcao = 0;


Action seguir_linha = () =>{
    print(1, $"seguindo linha: {velocidade}");

    if((millis() > update_time) && (velocidade < velocidade_max)){
		update_time = millis() + 16;
		velocidade++;
	}

    if(millis() - ultima_correcao > 1750){
        bc.onTF(1000, -1000);
        bc.wait(50);
        if(branco(0) && branco(1) && branco(2) && branco(3)){
            limpar_console();
            print(1, "SAÍ DA LINHA !@!21!");
            tempo_correcao = millis() + 350;
            while(tempo_correcao > millis()){
                if(preto(0) || preto(1) || preto(2) || preto(3)){
                    ultima_correcao = millis();
                    return;
                }
                bc.onTF(1000, -1000);
            }
            bc.onTF(-1000, 1000);
            bc.wait(350);
            tempo_correcao = millis() + 1750;
            while(tempo_correcao > millis()){
                if(!branco(0) || !branco(1) || !branco(2) || !branco(3)){
                    ultima_correcao = millis();
                    return;
                }
                bc.onTF(-1000, -1000);
            }

            ajustar_linha();

            travar();
        }
    }

    if(preto(1)){
        velocidade = velocidade_padrao;
        tempo_correcao = millis() + 190;
        while(tempo_correcao > millis()){
            if(branco(1) || preto(2)){
                break;
            }
            bc.onTF(-1000, 1000);
        }
        mover(velocidade, velocidade);
        delay(16);
        ultima_correcao = millis();
    }
    else if(preto(2)){
        velocidade = velocidade_padrao;
        tempo_correcao = millis() + 190;
        while(tempo_correcao > millis()){
            if(branco(2) || preto(1)){
                break;
            }
            bc.onTF(1000, -1000);
        }
        mover(velocidade, velocidade);
        delay(16);
        ultima_correcao = millis();
    }
    else{
        mover(velocidade, velocidade);
        if(preto(0) || preto(1) || preto(2) || preto(3)){
            ultima_correcao = millis();
        }
    }
};

Action verifica_curva = () =>{
    if(tem_linha(0) || cor(0) == "PRETO"){
        parar();
        led(0, 0, 0);
        print(1, "curva direita!");
        // verificar verde
        encoder(-250, 2f);
        ajustar_linha();
        // verificar verde
        encoder(250, 8);
        float objetivo = converter_graus(eixo_x() + 15);
        while(!intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
            mover(1000, -1000);
            if(tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3)){return;}
        }
        objetivo = converter_graus(eixo_x() + 90);
        while(!tem_linha(1) && !tem_linha(2)){
            mover(1000, -1000);
            if(intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                girar_esquerda(30);
                break;
            }
        }
        delay(190);
        ajustar_linha();
        calibrar();
        ultima_correcao = millis();
    }else if(tem_linha(3) || cor(3) == "PRETO"){
        parar();
        led(0, 0, 0);
        print(1, "curva esquerda!");
        // verificar verde
        encoder(-250, 2f);
        ajustar_linha();
        // verificar verde
        encoder(250, 8);
        float objetivo = converter_graus(eixo_x() - 15);
        while(!intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
            mover(-1000, 1000);
            if(tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3)){return;}
        }
        objetivo = converter_graus(eixo_x() - 90);
        while(!tem_linha(1) && !tem_linha(2)){
            mover(-1000, 1000);
            if(intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                girar_direita(30);
                break;
            }
        }
        delay(190);
        ajustar_linha();
        calibrar();
        ultima_correcao = millis();
    }
};


bool debug = false;

bc.actuatorSpeed(150);
bc.actuatorUp(100);

calibrar();

if(bc.angleActuator() >= 0 && bc.angleActuator() < 88){
    bc.actuatorSpeed(150);
    bc.actuatorUp(600);
}

while(!debug){
    verifica_calibrar();
    seguir_linha();
    verifica_curva();
}


while(debug){
    print(1, "DEBUG");
    print(2, bc.angleActuator());
}


