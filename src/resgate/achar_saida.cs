void entregar_vitima()
{
    abrir_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima())
    {
        if (millis() > timeout_vitima)
        {
            levantar_atuador();
            fechar_atuador();
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    levantar_atuador();
    fechar_atuador();
}

void achar_saida()
{
    float direcao_inicial = 0; // variavel para a posição inical do robô

    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo 

    direcao_saida = 0;      //inicia as localizações zeradas 
    direcao_triangulo = 0;

    alinhar_angulo();
    encoder(100, 10); // empurra possiveis bolinhas para frente
    encoder(-250, 10);
    abaixar_atuador();
    delay(511);
    abrir_atuador();
    encoder(250, 5);
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    alinhar_angulo();

    direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate

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

    alinhar_ultra(105); // move o robô até o ultrasonico frontal registrar 67cm para iniciar verificação do canto esquerdo
    fechar_atuador();
    levantar_atuador();
    delay(511);
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    parar();
    delay(64);
    alinhar_angulo();
    if (luz(4) < 2) // verifica se o triangula esta lá
    {
        direcao_triangulo = 1; // determina que o triangulo está a esquerda
        print(2, "TRIÂNGULO FRENTE");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            girar_esquerda(90);
            entregar_vitima();
            girar_direita(90);
            alinhar_ultra(26);
        }
        else
        {
            girar_direita(45);
            alinhar_ultra(26);
        }
        if (direcao_saida != 0) return;

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

    objetivo_direita((int)converter_graus(direcao_inicial + 90));
    if (!tem_vitima())
    {
        encoder(100, 10);
        encoder(-250, 10);
        alinhar_angulo();
        abaixar_atuador();
        abrir_atuador();
        encoder(250, 5);
    }
    alinhar_ultra(115);
    fechar_atuador();
    levantar_atuador();
    delay(511);
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
            girar_esquerda(90);
            entregar_vitima();
            girar_direita(90);
            alinhar_ultra(26);
        }
    }

    ler_ultra();
    while (ultra_frente < 65) // anda para tras procurando saida
    {
        mover(-250, -250);
        ler_ultra();
        if (ultra_esquerda > 300)
        {
            direcao_saida = direcao_inicial; // determina que a saida está na frente a direita
            print(1, "saida na frente direita");
            som("D3", 300);
            som("C3", 300);
            break;
        }
    }

    if (direcao_saida == 0) // se a saida ainda não foi encontrada ela está na ultima posição possivel
    {
        direcao_saida = converter_graus(direcao_inicial + 90); // determina que a saida está na direita
        print(1, "saida na frente direita");
        som("D3", 300);
        som("C3", 300);

    }
    if (direcao_triangulo == 0) // se o triangulo ainda não foi encontrado ele está na ultima possição possivel
    {
        direcao_triangulo = converter_graus(direcao_inicial + 135); // determina que o triangulo a direita
        print(2, "triangulo encontrado na frente direita");
        som("D3", 150);
        som("C3", 150);
    }
}
