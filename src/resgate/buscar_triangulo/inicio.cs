float direcao_inicial = 0; // variavel para a posição inical do robô

const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
            relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
            sense_triangulo = 10f; // constante de sensibilidade para encontrar triangulo

direcao_saida = 0;      //inicia as localizações zeradas 
direcao_triangulo = 0;

alinhar_angulo();
alinhar_angulo();
preparar_atuador();
alinhar_ultra(255); // vai para o inicio da sala de resgate 
alinhar_angulo();

direcao_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate

void primeira_busca()
{
    ler_ultra();
    // Busca o triângulo na direita
    while (ultra_frente > 180)
    {
        ler_ultra();
        mover(180, 180);
        if (ultra_direita > 300)  // Caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            direcao_saida = 3; // Determina que a saida está a direita
            print(1, "SAÍDA DIREITA");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // Realiza equação y = ax + b para identificar o triangulo de resgate
        {
            direcao_triangulo = 3; // Determina que o triangulo está a direita
            print(2, "TRIÂNGULO DIREITA");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }

    // Se alinha para verificar se o triângulo está na posição frontal
    mover(300, 300);
    delay(1500);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(105);
    alinhar_ultra(85);
    mover_tempo(200, 688);
    alinhar_angulo();

    if (luz(4) < 2) // Tenta detectar o triângulo com o sensor de luz da frente
    {
        direcao_triangulo = 1; // Determina que o triangulo está na posição frontal
        print(2, "TRIÂNGULO FRONTAL");
        som("D3", 150);
        som("C3", 150);
        if (tem_vitima())
        {
            // Se houver vítima no atuador, entrega ela e se alinha pra próxima ação
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
            // Se não houver, se alinha pra próxima ação
            girar_direita(45);
            alinhar_ultra(26);
        }
        if (direcao_saida != 0)
        {
            // Se já encontrou a saída, se alinha pra iniciar o resgate
            girar_direita(45);
            alinhar_ultra(124);
            alinhar_angulo();
            alinhar_ultra(124);
            girar_direita(90);
            int timeout = millis() + 400;
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
        // Se não achou o triângulo nessa posição, continua buscando
        segunda_busca();
    }
}

void segunda_busca()
{
    // Se posiciona pra continuar a busca pelo triângulo
    alinhar_angulo();
    girar_direita(45);

    ler_ultra();
    while (ultra_frente > 26)
    {
        ler_ultra();
        mover(200, 200);
        if (ultra_esquerda > 300 && direcao_saida == 0)
        {
            // Se o ultrassônico ler algo muito superior ao esperado
            direcao_saida = 1; // Determina que a saída está a esquerda
            print(1, "SAÍDA ESQUERDA");
            som("D3", 300);
            som("C3", 300);
        }
    }

    // Se alinha pra verificar se o triângulo está na posição frontal direita
    objetivo_direita(converter_graus(direcao_inicial + 90));
    preparar_atuador(true);
    mover(300, 300);
    fechar_atuador();
    levantar_atuador();
    alinhar_ultra(85);
    mover(200, 200);
    delay(700);
    alinhar_angulo();
    delay(64);

    if (direcao_triangulo == 0 && luz(4) < 2)
    {
        // Se ele ainda não tinha achado o triângulo e a leitura atual for < 2
        direcao_triangulo = 2; // Determina que o triângulo está na posição frontal direita
        print(2, "TRIÂNGULO FRONTAL DIREITA");
        som("D3", 150);
        som("C3", 150);
        // Se houver vítima no atuador, entrega ela e se alinha pra próxima ação
        if (tem_vitima())
        {
            encoder(-300, 1.5f);
            girar_direita(45);
            alinhar_ultra(65);
            girar_esquerda(90);
            entregar_vitima();
        }

    }

}
