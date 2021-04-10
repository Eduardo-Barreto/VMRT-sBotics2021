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
