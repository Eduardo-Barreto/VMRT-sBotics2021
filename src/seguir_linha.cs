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
