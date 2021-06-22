void alcancar_saida()
{
    mover(300, 300);
    delay(500);
    mover(-300, -300);
    delay(550);
    parar();
    abaixar_atuador();
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
        mover(300, 300);
    limpar_console();
    print(2, "Saindo!");
    som("C1", 100);
}

void sair()
{
    alinhar_angulo();
    bot.TurnLedOff();

    print(1, "BUSCANDO SAÍDA");

    alinhar_ultra(256);
    while (ultra(0) > 180)
    {
        print(2, "Verificando direita");
        mover(300, 300);
        if (ultra(1) > 300)
        {
            print(3, "Encontrada!");
            som("B2", 150);
            alinhar_ultra(224);
            som("C3", 150);
            girar_direita(90);
            alinhar_angulo();
            alcancar_saida();
            return;
        }
    }
    print(3, "Saida a direita não encontrada");
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
            alcancar_saida();
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
    alcancar_saida();
    return;
}
