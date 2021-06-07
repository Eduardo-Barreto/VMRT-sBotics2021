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
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
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
        print(1, "<b><color=#248f75>CURVA VERDE - Direita</color></b>");
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        ajustar_linha();
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F3", 100);
            tempo_correcao = millis() + 150;
            while (!(tem_linha(1)))
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            som("G3", 100);
            tempo_correcao = millis() + 150;
            while (cor(1) == "PRETO")
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            parar();
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_direita(25);
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
        print(1, "<b><color=#248f75>CURVA VERDE - Esquerda</color></b>");
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        ajustar_linha();
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F3", 100);
            tempo_correcao = millis() + 150;
            while (!(tem_linha(2)))
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            som("G3", 100);
            tempo_correcao = millis() + 150;
            while (cor(2) == "PRETO")
            {
                if (millis() > tempo_correcao)
                    break;
                mover(190, 190);
            }
            parar();
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_esquerda(25);
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

// Verificações de curvas no preto
bool verifica_curva()
{
    // Atualiza leituras de cores, verifica se está no verde e depois no preto
    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        parar();
        delay(64);
        ler_cor();
        if (vermelho(0)) { return false; }
        if (preto_curva_esq)
        {
            encoder(200, 3);
            return false;
        }
        if (verifica_saida()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        // Confirmações visuais e sonoras de que entrou na condição da curva
        print(1, "CURVA PRETO - Direita");
        led(0, 0, 0);
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        // Confirmada a curva, gira até encontrar uma linha ou passar do ângulo máximo
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) && !vermelho(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                /* Se chegar ao ângulo máximo, é uma curva com um gap no final
                Se alinha e arruma a curva de 90 somente com a referência de graus*/
                tempo_correcao = millis() + 1000;
                for (int i = 0; i < 10; i++)
                {
                    encoder(-300, 0.2f);
                    if (millis() > tempo_correcao)
                    {
                        break;
                    }
                }
                mover(-1000, 1000);
                delay(650);
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
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha(true);
        ajustar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar();
        delay(64);
        ler_cor();
        if (vermelho(3)) { return false; }
        if (preto_curva_dir)
        {
            encoder(200, 3);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Esquerda");
        led(0, 0, 0);
        som("C3", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) && !vermelho(2))
        {
            ler_cor();
            if (proximo(eixo_x(), objetivo))
            {
                tempo_correcao = millis() + 1000;
                for (int i = 0; i < 10; i++)
                {
                    encoder(-300, 0.2f);
                    if (millis() > tempo_correcao)
                    {
                        break;
                    }
                }
                mover(1000, -1000);
                delay(650);
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
        ajustar_linha(true);
        ajustar_linha(true);
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
