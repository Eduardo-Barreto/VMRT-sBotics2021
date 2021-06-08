void achar_saida()
{
    float pos_inicial = 0; // variavel para a posição inical do robô
    const float relacao_sensores_a = -1.0102681118083f,   // constante A da equação para achar o triangulo de resgate
                relacao_sensores_b = 401.7185510553336f,  // constante B da equação para achar o triangulo de resgate
                sense_triangulo = 0.5f; // constande de sensibilidade para encontrar triangulo 

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
            pos_saida = pos_inicial + 90; // determina que a saida está à direita
            print(2, "saida na direita encontrada");
        }
        else if (proximo(ultra_direita, (ultra_frente * relacao_sensores_a) + relacao_sensores_b, sense_triangulo)) // realiza equação y = ax + b para identificar o triangulo de resgate
        {
            pos_triangulo = pos_inicial + 90; // determina que o triangula está a direita
            print(3, "triangulo encontrado");
            travar();
        }
    }
    /*   print(3, "Saida a direita não encontrada");
      som("D3", 150);
      som("C3", 150);

      alinhar_ultra(100);
      limpar_console();
      print(1, "BUSCANDO SAÍDA");
      girar_direita(45);
      while (ultra(0) > 103)
      {
          print(2, "Verificando esquerda");
          mover(300, 300);
          if (ultra(2) > 256)
          {
              print(3, "Encontrada!");
              som("B2", 150);
              girar_esquerda(45);
              alinhar_ultra(32);
              som("C3", 150);
              girar_esquerda(45);
              encoder(300, 4);
              girar_esquerda(45);
              alinhar_angulo();
              return;
          }
      }
      print(3, "Saida a esquerda não encontrada");
      som("D3", 150);
      som("C3", 150);

      girar_direita(45);
      limpar_console();
      print(2, "indo para saida a frente na direita");
      alinhar_angulo();
      alinhar_ultra(32);
      girar_esquerda(45);
      encoder(300, 4);
      girar_esquerda(45);
      alinhar_angulo();
      return; */
}