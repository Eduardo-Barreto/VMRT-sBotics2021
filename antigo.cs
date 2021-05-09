/*Last change: 10/04/2021 | 16:56:45
----------------------------------------------------------------------------------------------------*/

// Funções de cálculo
void Main(){
Func<float, float, float, float, float, int> map = delegate(float val, float minimo, float maximo, float minimoSaida, float maximoSaida) {
	// "mapeia" ou reescala um val(val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
	return (int)( (((val - minimo) * (maximoSaida - minimoSaida)) / (maximo - minimo)) + minimoSaida);
};

Func<float, float> converter_graus= (graus) => {
	// converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
	float graus_convertidos = graus;
	graus_convertidos = (graus_convertidos < 0) ? 360 + graus_convertidos : graus_convertidos;
	graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
	graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
	return graus_convertidos;
};

// Verifica se um val(val) esta entre um val mínimo(minimo) e máximo(maximo)
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
   sbyte vermelho = (sbyte)(map(valVermelho, 0, RGB, 0, 100));
   sbyte verde = (sbyte)(map(valVerde, 0, RGB, 0, 100));
   sbyte azul = (sbyte)(map(valAzul, 0, RGB, 0, 100));
   return ((vermelho < mediaVermelho) && (verde < mediaVerde) && (azul > mediaAzul));
};

Func <int, bool> tem_verde = (sensor) => {
    float valVermelho = bc.returnRed(sensor);
    float valVerde = bc.returnGreen(sensor);
    float valAzul = bc.returnBlue(sensor);
    byte mediaVermelho = 20, mediaVerde = 65, mediaAzul = 14;
    int RGB = (int)(valVermelho + valVerde + valAzul);
    sbyte vermelho = (sbyte)(map(valVermelho, 0, RGB, 0, 100));
	sbyte verde = (sbyte)(map(valVerde, 0, RGB, 0, 100));
	sbyte azul = (sbyte)(map(valAzul, 0, RGB, 0, 100));
    return ((vermelho < mediaVermelho) && (verde > mediaVerde) && (verde < 95) && (azul < mediaAzul));
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


Action levantar_atuador = () => {
    bc.actuatorSpeed(150);
    bc.actuatorUp(100);
    if(bc.angleActuator() >= 0 && bc.angleActuator() < 88){
        bc.actuatorSpeed(150);
        bc.actuatorUp(600);
    }
};


Func<bool> beco = () => {
    if((tem_verde(0) || tem_verde(1)) && (tem_verde(3) || tem_verde(4))){
        print(1, "BECO");
        ajustar_linha();
        if((tem_verde(0) || tem_verde(1)) && (tem_verde(3) || tem_verde(4))){
            print(2, "CONFIRMADO");
            led(0, 255, 0);
            girar_direita(170);
            float objetivo = converter_graus(eixo_x() + 15);
            while(!intervalo(eixo_x(), objetivo-1, objetivo+1)){
                bc.onTF(-1000, 1000);
                if(tem_linha(1)){
                    delay(150);
                    break;
                }
            }
            encoder(-250, 3);
            ajustar_linha();
            calibrar();
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
            update_time = millis() + 300;
            print(2, "");
            return true;
        }
    }
    print(2, "");
    return false;
};


Func<bool> verifica_verde = () =>{

    if(beco()){return true;}

    if(tem_verde(0) || tem_verde(1)){
        if(beco()){return true;}
        print(1, "CURVA VERDE - Direita");
        ajustar_linha();
        if(tem_verde(0) || tem_verde(1)){
            if(beco()){return true;}
            print(2, "CONFIRMADA");
            led(0, 255, 0);
            encoder(250, 14);
            girar_direita(15);
            float objetivo = converter_graus(eixo_x() + 75);
            while(!intervalo(eixo_x(), objetivo-1, objetivo+1)){
                bc.onTF(-1000, 1000);
                if(tem_linha(1)){
                    delay(150);
                    break;
                }
            }
            encoder(-250, 3);
            ajustar_linha();
            calibrar();
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
            update_time = millis() + 300;
            print(2, "");
            return true;
        }
    }
    if(tem_verde(3) || tem_verde(4)){
        if(beco()){return true;}
        print(1, "CURVA VERDE - Esquerda");
        ajustar_linha();
        if(tem_verde(3) || tem_verde(4)){
            if(beco()){return true;}
            print(2, "CONFIRMADA");
            led(0, 255, 0);
            encoder(250, 14);
            girar_esquerda(15);
            float objetivo = converter_graus(eixo_x() - 75);
            while(!intervalo(eixo_x(), objetivo-1, objetivo+1)){
                bc.onTF(1000, -1000);
                if(tem_linha(2)){
                    delay(150);
                    break;
                }
            }
            encoder(-250, 3);
            ajustar_linha();
            calibrar();
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
            update_time = millis() + 300;
            print(2, "");
            return true;
        }
    }
    return false;
};

Action verifica_curva = () =>{

    if(tem_linha(0) || cor(0) == "PRETO" || preto(0)){
        parar();
        print(1, "curva direita!");
        if(verifica_verde()){return;}
        encoder(-250, 2f);
        ajustar_linha();
        led(0, 0, 0);
        if(verifica_verde()){return;}
        encoder(250, 9);
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
        velocidade = velocidade_padrao;
        update_time = millis() + 300;
    }else if(tem_linha(3) || cor(3) == "PRETO" || preto(3)){
        parar();
        print(1, "curva esquerda!");
        if(verifica_verde()){return;}
        encoder(-250, 2f);
        ajustar_linha();
        led(0, 0, 0);
        if(verifica_verde()){return;}
        encoder(250, 9);
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
        velocidade = velocidade_padrao;
        update_time = millis() + 300;
    }
};


Action seguir_linha = () =>{
    print(1, $"seguindo linha: {velocidade}");
    bc.turnLedOff();

    if((millis() > update_time) && (velocidade < velocidade_max)){
		update_time = millis() + 32;
		velocidade++;
	}

    if((millis() - ultima_correcao > 1500)){
        bc.onTF(1000, -1000);
        bc.wait(50);
        if(branco(0) && branco(1) && branco(2) && branco(3)){
            limpar_console();
            print(1, "SAÍ DA LINHA !@!21!");
            tempo_correcao = millis() + 350;
            while(tempo_correcao > millis()){
                if(preto(0) || preto(1) || preto(2) || preto(3)){
                    ajustar_linha();
                    ultima_correcao = millis();
                    return;
                }
                bc.onTF(1000, -1000);
            }
            bc.onTF(-1000, 1000);
            bc.wait(350);
            tempo_correcao = millis() + 1750;
            while(tempo_correcao > millis()){
                //TODO: microverificacoes com curvas
                if(preto(0) || preto(1) || preto(2) || preto(3)){
                    ultima_correcao = millis();
                    return;
                }
                bc.onTF(-1000, -1000);
            }
            delay(150);

            ajustar_linha();
            ultima_correcao = millis();
            velocidade = velocidade_padrao;
            update_time = millis() + 300;
        }
    }

    if(tem_azul(1) || tem_azul(2)){
        while(tem_azul(0) || tem_azul(1) || tem_azul(2) || tem_azul(3)){
            bc.onTF(-1000, -1000);
        }
        tempo_correcao = millis() - ultima_correcao;
        while(tempo_correcao > millis()){
            //TODO: microverificacoes com curvas
            if(preto(0) || preto(1) || preto(2) || preto(3)){
                ultima_correcao = millis();
                return;
            }
            bc.onTF(-1000, -1000);
        }
        delay(150);

        ajustar_linha();
        ultima_correcao = millis();
        velocidade = velocidade_padrao;
        update_time = millis() + 300;
    }

    if(preto(1)){
        verifica_curva();
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
        verifica_curva();
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
        verifica_curva();
    }
};


bool debug = false;

calibrar();


while(!debug){
    verifica_calibrar();
    seguir_linha();
    verifica_curva();
}


while(debug){
    print(1, $"{tem_azul(0)} | {tem_azul(1)} | {tem_azul(2)} | {tem_azul(3)}");
}


}