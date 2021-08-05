void achar_saida()
{
    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo

    direcao_saida = 0;      //inicia as localizações zeradas 
    direcao_triangulo = 0;

    totozinho();
    encoder(300, 3);
    alinhar_angulo();
    preparar_atuador();
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    alinhar_angulo();

    direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate
    ler_ultra();
    while (ultra_frente > 180) // enqunto estiver a mais de 180cm da parede frontal busca por saida ou triangulo
    {
        ler_ultra();
        mover(180, 180);
        if (ultra_direita > 300)  // caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = 3; // determina que a saida está a direita
            print(1, "SAÍDA DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = 3; // determina que o triangulo está a direita
            print(2, "TRIÂNGULO DIREITA");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }
    mover(300, 300);
    delay(1500);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(105); // move o robô até o ultrasonico frontal registrar 67cm para iniciar verificação do canto esquerdo
    alinhar_ultra(85);
    mover_tempo(200, 688);
    alinhar_angulo();
    if (luz(4) < 2) // verifica se o triangulo esta lá
    {
        direcao_triangulo = 1; // determina que o triangulo está a esquerda
        print(2, "TRIÂNGULO FRONTAL");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            objetivo_esquerda(converter_graus(direcao_inicial - 45));
            entregar_vitima();
            girar_direita(90);
            alinhar_ultra(26);
        }
        else
        {
            girar_direita(45);
            alinhar_ultra(26);
        }
        if (direcao_saida != 0)
        {
            girar_direita(45);
            alinhar_ultra(124);
            alinhar_angulo();
            alinhar_ultra(124);
            girar_direita(90);
            timeout = millis() + 400;
            while (!toque())
            {
                mover(-300, -300);
                if (millis() > timeout)
                {
                    break;
                }
            }
            parar();
            return;
        };

    }

    else
    {
        alinhar_angulo();
        girar_direita(45); // vira 45º para efetuar verificação com ultrasonico lateral
        ler_ultra();

        while (ultra_frente > 26) // enqunto estiver a mais de 26cm da parede frontal busca por saida
        {
            ler_ultra();
            mover(200, 200);
            if (ultra_esquerda > 300 && direcao_saida == 0) // caso o ultrasonico da lateral esquerda veja uma distancia muito grande o robô encontrou a saida
            {
                direcao_saida = 1; // determina que a saida está a esquerda
                print(1, "SAÍDA ESQUERDA");
                som("D3", 300);
                som("C3", 300);
            }
        }
    }

    objetivo_direita(converter_graus(direcao_inicial + 90));
    preparar_atuador(true);
    mover(300, 300);
    delay(650);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    alinhar_angulo();
    delay(64);

    if (luz(4) < 2 && direcao_triangulo == 0)
    {
        direcao_triangulo = 2; // determina que o triangulo está a direita na frente
        print(2, "TRIANGULO FRONTAL DIREITA");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            objetivo_esquerda(converter_graus(direcao_inicial + 45));
            entregar_vitima();
        }
    }

    ler_ultra();
    while (ultra_frente < 65) // anda para tras procurando saida
    {
        mover(-250, -250);
        ler_ultra();
        if (ultra_esquerda > 300)
        {
            direcao_saida = 2; // determina que a saida está na frente a direita
            print(1, "SAIDA FRONTAL DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
    }

    if (direcao_saida == 0) // se a saida ainda não foi encontrada ela está na ultima posição possivel
    {
        direcao_saida = 3; // determina que a saida está a direita
        print(1, "SAÍDA DIREITA");
        som("D3", 300);
        som("C3", 300);

    }
    if (direcao_triangulo == 0) // se o triangulo ainda não foi encontrado ele está na ultima possição possivel
    {
        direcao_triangulo = 3; // determina que o triangulo está a direita
        print(2, "TRIÂNGULO DIREITA");
        som("D3", 150);
        som("C3", 150);
    }

    if (direcao_triangulo == 1 || direcao_triangulo == 2)
    {
        mover(-300, -300);
        delay(300);
        girar_esquerda(5);
        objetivo_direita(converter_graus(direcao_inicial + 90));
        alinhar_ultra(124);
        alinhar_angulo();
        alinhar_ultra(124);
        girar_direita(90);
        alinhar_angulo();
        mover(-300, -300);
        delay(500);
        alinhar_angulo();
        timeout = millis() + 1000;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
        delay(300);
        timeout = millis() + 1000;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
    }
    else
    {
        objetivo_direita(converter_graus(direcao_inicial + 180));
        alinhar_angulo();
        alinhar_ultra(124);
        alinhar_angulo();
        alinhar_ultra(124);
        girar_direita(90);
        alinhar_angulo();
        mover(-300, -300);
        delay(750);
        alinhar_angulo();
        timeout = millis() + 1000;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
        delay(300);
        timeout = millis() + 1000;
        while (!toque())
        {
            mover(-300, -300);
            if (millis() > timeout)
            {
                parar();
                break;
            }
        }
        alinhar_angulo();
    }
}
