float   velocidade_padrao = 150,
        velocidade = 200,
        velocidade_max = 300,
        update_time = millis() + 16;

uint    tempo_correcao = 0,
        ultima_correcao = 0;

Action<string> encruzilhada = (direcao) =>{
    encoder(-250, 2);
    ajustarLinha();
    // verifica verde
    Func <string, bool> verifica_curva = (lado) => {
        encoder(250, 10);
        if(lado == "direita"){
            float objetivo = converter_graus(eixo_x() + 15);
            while(!intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                mover(1000, -1000);
                if(preto(0) || preto(1)){return false;}
            }
        }else{
            float objetivo = converter_graus(eixo_x() - 15);
            while(!intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                mover(-1000, 1000);
                if(preto(0) || preto(1)){return false;}
            }
        }
        return true;
    };

    if(!verifica_curva(direcao)){return;}
    if(direcao == "direita"){
        float objetivo = converter_graus(eixo_x() + 90);
        while(!tem_linha(0) && !tem_linha(1)){
            mover(1000, -1000);
            if(intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                girar_esquerda(35);
                encoder(200, 3);
                girar_direita(15);
                break;
            }
        }
    }else{
        float objetivo = converter_graus(eixo_x() - 90);
        while(!tem_linha(0) && !tem_linha(1)){
            mover(-1000, 1000);
            if(intervalo(eixo_x(), (objetivo-1), (objetivo+1))){
                girar_direita(35);
                encoder(200, 3);
                girar_esquerda(15);
                break;
            }
        }
    }

    ajustarLinha();
    calibrar();
};


Action seguir_linha = () =>{
    led(0, 0, 0);
    print(1, $"seguindo linha: {velocidade}");
    print(2, millis() - ultima_correcao);

    if((millis() > update_time) && (velocidade < velocidade_max)){
		update_time = millis() + 16;
		velocidade++;
	}

    if(preto(0)){
        tempo_correcao = millis() + 200;
        while(millis() < tempo_correcao){
            if(preto(1)){break;}
            mover(1000, -1000);
        }
        mover(velocidade_padrao, velocidade_padrao);
        delay(16);
        if(luz(0) < 55){
            encruzilhada("direita");
        }
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }
    else if(preto(1)){
        tempo_correcao = millis() + 200;
        while(millis() < tempo_correcao){
            if(preto(0)){break;}
            mover(-1000, 1000);
        }
        mover(velocidade_padrao, velocidade_padrao);
        delay(16);
        if(luz(1) < 55){
            encruzilhada("esquerda");
        }
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }
    else{
        mover(velocidade, velocidade);
        print(1, $"Seguindo linha com {velocidade}");
    }
};
