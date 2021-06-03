// Comandos úteis para todo o código

float map(float val, float minimo, float maximo, float minimoSaida, float maximoSaida)
{
    //"mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo, float sensibilidade = 1)
{
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo - sensibilidade && atual < objetivo + sensibilidade);
}

float converter_graus(float graus)
{
    // converte os graus pra sempre se manterem entre 0~360, uso em calculos para curvas
    float graus_convertidos = graus;
    graus_convertidos = (graus_convertidos < 0) ? (360 + graus_convertidos) : graus_convertidos;
    graus_convertidos = (graus_convertidos > 360) ? (graus_convertidos - 360) : graus_convertidos;
    graus_convertidos = (graus_convertidos == 360) ? 0 : graus_convertidos;
    return graus_convertidos;
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bc.actuatorSpeed(150);
    bc.actuatorUp(100);
    if (bc.angleActuator() >= 0 && bc.angleActuator() < 88)
    {
        bc.actuatorSpeed(150);
        bc.actuatorUp(600);
    }
}
