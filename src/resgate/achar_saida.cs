/* void alcancar_saida()
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
} */

void achar_saida()
{
    alinhar_angulo();
    alinhar_ultra(255); // vai para o inicio da sala de resgate 
    alinhar_angulo();
    travar();
    float pos_inicial = eixo_x();

    while (ultra(0) > 180) //enqunto estiver a mais de 180cm da parede frontal busca por saida ou triangulo
    {
        mover(100, 100);
        if (ultra(1) > 300) //caso o ultrasonico da lateral direita veja uma distancia muito grande o robô encontrou a saida
            pos_saida = 90;

        else if (ultra(1) > 1) ;
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