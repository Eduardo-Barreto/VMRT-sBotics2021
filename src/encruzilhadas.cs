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