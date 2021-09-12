void mover_para(int posI, int posF)
{
    float anguloObjetivo = 0,
    distanciaAlinhamento = 0;

    int posIx = 0,
         posIy = 0,
         posFx = 0,
         posFy = 0,
         objX = 0,
         objY = 0;

    switch (posI)
    {
        case 0:
            posIx = 3;
            posIy = 1;
            break;
        case 1:
            posIx = 3;
            posIy = 2;
            break;
        case 2:
            posIx = 3;
            posIy = 3;
            break;
        case 3:
            posIx = 3;
            posIy = 4;
            break;
        case 4:
            posIx = 2;
            posIy = 4;
            break;
        case 5:
            posIx = 1;
            posIy = 4;
            break;
        case 6:
            posIx = 1;
            posIy = 3;
            break;
        case 7:
            posIx = 1;
            posIy = 2;
            break;
        case 8:
            posIx = 1;
            posIy = 1;
            break;
        case 9:
            posIx = 2;
            posIy = 1;
            break;
    }
    switch (posF)
    {
        case 0:
            posFx = 3;
            posFy = 1;
            break;
        case 1:
            posFx = 3;
            posFy = 2;
            break;
        case 2:
            posFx = 3;
            posFy = 3;
            break;
        case 3:
            posFx = 3;
            posFy = 4;
            break;
        case 4:
            posFx = 2;
            posFy = 4;
            break;
        case 5:
            posFx = 1;
            posFy = 4;
            break;
        case 6:
            posFx = 1;
            posFy = 3;
            break;
        case 7:
            posFx = 1;
            posFy = 2;
            break;
        case 8:
            posFx = 1;
            posFy = 1;
            break;
        case 9:
            posFx = 2;
            posFy = 1;
            break;
    }

    objX = posFx - posIx;
    objY = posFy - posIy;

    anguloObjetivo = (float)(Math.Atan2(objX, objY) * (180 / Math.PI));
    anguloObjetivo = converter_graus(direcao_inicial + anguloObjetivo);

    levantar_atuador();

    if (((Math.Abs(eixo_x() - anguloObjetivo)) > 180))
    {
        objetivo_direita(anguloObjetivo);
    }
    else
    {
        objetivo_esquerda(anguloObjetivo);
    }

    distanciaAlinhamento = (float)(50 / (Math.Cos((Math.Atan2(objX, objY)))));
    alinhar_ultra(((float)Math.Abs(distanciaAlinhamento)) - 10);

}