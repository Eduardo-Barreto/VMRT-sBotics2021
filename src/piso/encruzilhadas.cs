bool falso_verde()
{
    /*
    Falso Verde: Verifica se o robô realmente está no verde e não passou reto de uma outra encruzilhada
        Vem da verificação do verde
        Define um tempo máximo de verificação de 200 milissegundos
        Enquanto está nesse tempo:
            Anda para trás
            Se encontrar a cor preta, vai para frente e retorna verdadeiro (era realmente um falso verde)
            Senão, continua a movimentação, retorna falso (falso falso verde = verde verdadeiro), e realiza a curva
    */
    parar();
    int tempo_check_preto = millis() + 200;
    while (millis() < tempo_check_preto)
    {
        mover(-180, -180);
        if (cor(0) == "PRETO" || cor(3) == "PRETO")
        {
            mover_tempo(300, 288);
            return true;
        }
    }
    mover(200, 200);
    delay(196);
    parar();
    return false;
}

bool beco()
{
    /*
    Beco: Verifica se o robô está no Beco sem saída (verde dos dois lados da encruzilhada)
        Para o robô por tempo suficiente para atualizar a leitura dos sensores
        Se detectar verde dos dois lados:
            Ajusta na linha e para novamente por tempo suficiente para atualizar a leitura dos sensores
            Se novamente identificar verde dos dois lados
                Verifica se é realmente uma encruzilhada (falso_verde())
                Confirmado o beco, acende o led verde e escreve no console
                Indica pelo som que caiu na condição correta
                Vai para frente e faz uma curva de 170 graus para a direita
                Gira até encontrar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Para, lê as cores e verifica se está na condição do beco
    parar(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        print_luz_marker();
        alinhar_linha();
        delay(64);
        ler_cor();
        print_luz_marker();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            print_luz_marker();
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição
            console_led(1, "<:<b>BECO SEM SAÍDA</b>:>", "verde");
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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
    /*
    Verifica Verde: Verifica se o robô está em uma encruzilhada com verde
        Atualiza as leituras dos sensores
        Se encontrar verde em algum dos sensores da direita
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à direita
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar verde em algum dos sensores da esquerda
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à esquerda
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        print_luz_marker();
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha e verifica novamente
        alinhar_linha();
        delay(64);
        alinhar_linha();
        ler_cor();
        print_luz_marker();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Direita", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 447);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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

    // Verifica os sensores da esquerda
    else if (verde2 || verde3)
    {
        print_luz_marker();
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha e verifica novamente
        alinhar_linha();
        delay(64);
        alinhar_linha();
        ler_cor();
        print_luz_marker();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Esquerda", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 447);
            girar_esquerda(30);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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
    /*
    Verifica Curva: Verifica se o robô está em alguma curva de 90° no preto
        Atualiza as leituras dos sensores
        Verifica se está no verde ou no fim da arena
        Se encontrar preto no sensor da direita
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
            Atualiza a leitura dos sensores
            Se estiver no vermelho (fim da arena), retorna falso (não é curva)
            Se encontrar preto no sensor da esquerda (encruz reta)
                Vai para frente e retorna falso (não é curva)
            Verifica novamente pela saída da arena (vermelho) e verde
            Vai para trás e verifica novamente pelo verde
            Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
            Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
            Com a curva totalmente confirmada, continua girando até achar a linha
            Se passar de 115 graus, asssume que é o ladrilho de Curva C com GAP
                Se alinha atrás e por graus
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
            Se alinha na linha
            Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar preto no sensor da esquerda
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
                Atualiza a leitura dos sensores
                Se estiver no vermelho (fim da arena), retorna falso (não é curva)
                Se encontrar preto no sensor da direita (encruz reta)
                    Vai para frente e retorna falso (não é curva)
                Verifica novamente pela saída da arena (vermelho) e verde
                Vai para trás e verifica novamente pelo verde
                Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
                Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
                Com a curva totalmente confirmada, continua girando até achar a linha
                Se passar de 115 graus, assume que é o ladrilho de Curva C com GAP
                    Se alinha atrás e por graus
                    Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
                Se alinha na linha
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza leituras de cores, verifica se está no verde e depois no vermelho

    if (millis() < update_curva) { return false; }

    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        parar(64);
        ler_cor();
        print_luz_marker();
        if (vermelho(0) || colorido(0))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_esq)
        {
            mover_tempo(300, 288);
            return false;
        }
        if (verifica_saida()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        int timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        // Feedbacks visuais e sonoross de que entrou na condição da curva
        console_led(1, "<b>CURVA PRETO</b> - Direita", "preto");
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        mover_tempo(300, 351);
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
        while (!tem_linha(1) || vermelho(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                /* Se chegar ao ângulo máximo, é uma curva com um gap no final
                Se alinha e arruma a curva de 90 somente com a referência de graus*/
                mover_tempo(-300, 239);
                mover(-1000, 1000);
                delay(650);
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(1000, -1000);
        }
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        alinhar_linha();
        encoder(-300, 2);
        alinhar_linha(true);
        alinhar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar(64);
        ler_cor();
        print_luz_marker();
        if (vermelho(3) || colorido(3))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_dir)
        {
            mover_tempo(300, 288);
            return false;
        }
        if (verifica_saida()) { return false; }
        int timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        console_led(1, "<b>CURVA PRETO</b> - Esquerda", "preto");
        som("C3", 100);
        mover_tempo(300, 351);
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
        while (!tem_linha(2) || vermelho(2))
        {
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 239);
                mover(1000, -1000);
                delay(650);
                alinhar_angulo();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(-1000, 1000);
        }
        delay(200);
        alinhar_linha();
        encoder(-300, 2);
        alinhar_linha(true);
        alinhar_linha(true);
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
