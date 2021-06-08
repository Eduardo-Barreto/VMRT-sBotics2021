void achar_saida()
{

    float pos_inicial = 0, // variavel para a posição inical do robô
          ultima_leitura = 0; // variavel de ultima leitura do sensor ultrasonico

    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 1.5f; // constande de sensibilidade para encontrar triangulo 

    pos_saida = 0;      //inicia as localizações zeradas 
    pos_triangulo = 0;

    alinhar_angulo();
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    alinhar_angulo();

    pos_inicial = eixo_x(); // define a posição em que o robô estava ao entrar na sala de resgate

    while (ultra(0) > 185) // enqunto estiver a mais de 185cm da parede frontal busca por saida ou triangulo
    {
        mover(200, 200);
        ler_ultra();
        if (ultra_direita > 300)  // caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
        {
            pos_saida = pos_inicial + 90; // determina que a saida está a direita
            print(1, "saida na direita encontrada");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            pos_triangulo = pos_inicial + 90; // determina que o triangulo está a direita
            print(2, "triangulo encontrado na direita");
            som("D3", 150);
            som("C3", 150);
            break;
        }
    }

    alinhar_ultra(100); // move o robô até o ultrasonico frontal registrar 100cm para iniciar verificação do canto esquerdo
    girar_direita(45); // vira 45º para efetuar verificação com ultrasonico lateral
    ler_ultra();

    while (ultra_frente > 103) // enqunto estiver a mais de 103cm da parede frontal busca por saida ou triangulo 
    {
        mover(200, 200);
        ler_ultra();
        if (ultra_esquerda > 300) // caso o ultrasonico da lateral esquerda veja uma distancia muito grande o robô encontrou a saida
        {
            pos_saida = pos_inicial - 90; // determina que a saida está a esquerda
            print(1, "saida na esquerda encontrada");
            som("D3", 300);
            som("C3", 300);
            break;
        }
        else if (proximo(ultra_esquerda, ultima_leitura, 1)) // se o robô estiver em parelo ao triangulo a leitura do ultrasonico esquerdo tera apenas pequenas variações
        {
            pos_triangulo = pos_inicial - 90; // determina que o triangulo está a esquerda
            print(2, "triangulo encontrado na esquerda");
            som("D3", 150);
            som("C3", 150);
            break;
        }
        ultima_leitura = ultra(2); // le o ultrasonico esquerdo e espera 128 milisegundos para efetuar o proximo ciclo assim dando possibilidade para alteração no valor
        delay(128);
    }
    if ((pos_saida == 0) && (pos_triangulo == 0)) // caso nada tenha sido encontrado houve um erro no processo
    {
        print(3, "erro saida e triangulo não encontrados");
        som("C3", 1000);
    }
    if (pos_saida == 0) // se a saida ainda não foi encontrada ela está na ultima posição possivel
    {
        pos_saida = pos_inicial; // determina que a saida está na frente a direita
        print(1, "saida na frente direita");
        som("D3", 300);
        som("C3", 300);

    }
    if (pos_triangulo == 0) // se o triangulo ainda não foi encontrado ele está na ultima possição possivel
    {
        pos_triangulo = pos_inicial; // determina que o triangulo está na frente a direita
        print(2, "triangulo encontrado na frente direita");
        som("D3", 150);
        som("C3", 150);
    }
}