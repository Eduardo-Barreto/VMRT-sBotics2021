void alcancar_saida()
{
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
        mover(300, 300);
    limpar_console();
    print(2, "Saindo!");
}

void sair()
{

    alinhar_angulo();
    bc.TurnLedOff();

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
    print(2, "Saida a direita não encontrada");

    alinhar_ultra(100);
    girar_direita(45);
    while (ultra(0) > 103)
    {
        print(2, "Verificando esquerda");
        mover(300, 300);
        if (ultra(2) > 256)
        {
            print(3, "Encontrada!");
            girar_esquerda(45);
            alinhar_ultra(32);
            girar_esquerda(45);
            encoder(300, 4);
            girar_esquerda(45);
            alinhar_angulo();
            alcancar_saida();
            return;
        }
    }

    girar_direita(45);
    alinhar_angulo();
    alinhar_ultra(32);
    girar_esquerda(45);
    encoder(300, 4);
    girar_esquerda(45);
    alinhar_angulo();
    alcancar_saida();
    return;


}
