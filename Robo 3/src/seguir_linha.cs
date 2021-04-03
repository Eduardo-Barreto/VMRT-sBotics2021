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
