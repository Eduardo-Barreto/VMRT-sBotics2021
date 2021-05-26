bool beco()
{
    parar();
    delay(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        print(1, "BECO SEM SAÍDA");
        led(0, 255, 0);
        som("F#", 100);
        som("D", 100);
        som("F#", 100);
        som("D", 100);
        encoder(300, 12);
        girar_direita(170);
        while (!tem_linha(1))
        {
            mover(1000, -1000);
            if (angulo_reto())
            {
                break;
            }
        }
        delay(200);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }
    return false;
}

bool verifica_verde()
{
    ler_cor();
    if (verde0 || verde1)
    {
        if (beco()) { return true; }
        print(1, "CURVA VERDE - Direita");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            if (beco()) { return true; }
            led(0, 255, 0);
            som("G", 100);
            while (!(tem_linha(1)))
            {
                mover(190, 190);
            }
            som("A", 100);
            while (cor(1) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("B", 100);
            encoder(300, 10);
            girar_direita(20);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        else
        {
            return false;
        }
    }

    else if (verde2 || verde3)
    {
        if (beco()) { return true; }
        print(1, "CURVA VERDE - Esquerda");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            if (beco()) { return true; }
            led(0, 255, 0);
            som("G", 100);
            while (!(tem_linha(2)))
            {
                mover(190, 190);
            }
            som("A", 100);
            while (cor(2) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("B", 100);
            encoder(300, 10);
            girar_esquerda(20);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}


bool verifica_curva()
{
    ler_cor();
    if (verifica_verde()) { return true; }

    else if (preto_curva_dir)
    {
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Direita");
        led(0, 0, 0);
        som("C", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            ler_cor();
            if (preto1 || preto2)
            {
                return false;
            }
            mover(1000, -1000);
        }
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1))
        {
            ler_cor();
            if (proximo(eixo_x(), objetivo))
            {
                encoder(-300, 7f);
                mover(-1000, 1000);
                delay(300);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(1000, -1000);
        }
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Esquerda");
        led(0, 0, 0);
        som("C", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            ler_cor();
            if (preto1 || preto2)
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2))
        {
            ler_cor();
            if (proximo(eixo_x(), objetivo))
            {
                encoder(-300, 7f);
                mover(1000, -1000);
                delay(300);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(-1000, 1000);
        }
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }
    else
    {
        return false;
    }
}
