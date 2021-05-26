// Verificação do beco sem saída
bool beco()
{
    // Para, lê as cores e verifica se está na condição do beco
    parar();
    delay(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        ajustar_linha();
        delay(64);
        ler_cor();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            // Feedback visual e sonoro para indicar que entrou na condição
            print(1, "BECO SEM SAÍDA");
            led(0, 255, 0);
            som("F#", 100);
            som("D", 100);
            som("F#", 100);
            som("D", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
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
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        return false;
    }
    return false;
}

// Verificação das condições de verde
bool verifica_verde()
{
    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "CURVA VERDE - Direita");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F", 100);
            while (!(tem_linha(1)))
            {
                mover(190, 190);
            }
            som("G", 100);
            while (cor(1) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("A", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
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
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }

    // Verifica os sensores da direita
    else if (verde2 || verde3)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "CURVA VERDE - Esquerda");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F", 100);
            while (!(tem_linha(2)))
            {
                mover(190, 190);
            }
            som("G", 100);
            while (cor(2) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("A", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
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
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
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
            if ((tem_linha(1) || tem_linha(2)) && !azul(1) && !azul(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) && !azul(1))
        {
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
            if ((tem_linha(1) || tem_linha(2)) && !azul(1) && !azul(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) && !azul(2))
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
