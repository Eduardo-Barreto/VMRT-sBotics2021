// Declaração das variáveis principais de todo o projeto, separadas por tipos
float saida1 = 0,
        saida2 = 0,
        media_meio = 0,
        media_fora = 0;

int velocidade_padrao = 190,
        velocidade = 200,
        velocidade_max = 300,
        update_time = 16,
        tempo_correcao = 0,
        ultima_correcao = 0;

bool preto0 = false,
        preto1 = false,
        preto2 = false,
        preto3 = false,

        verde0 = false,
        verde1 = false,
        verde2 = false,
        verde3 = false,

        preto_curva_dir = false,
        preto_curva_esq = false;

int[] angulos_retos = { 0, 90, 180, 270 };
// Comandos úteis para todo o código

float map(float val, float minimo, float maximo, float minimoSaida, float maximoSaida)
{
    //"mapeia" ou reescala um val (val), de uma escala (minimo~maximo) para outra (minimoSaida~maximoSaida)
    return (val - minimo) * (maximoSaida - minimoSaida) / (maximo - minimo) + minimoSaida;
}

bool proximo(float atual, float objetivo)
{
    // Verifica se um val (atual) esta próximo de um objetivo (objetivo)
    return (atual > objetivo - 1 && atual < objetivo + 1);
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
// Métodos de leitura e outros

int millis() => (int)(bc.Timer());
string cor(int sensor) => bc.ReturnColor(sensor);
int luz(byte sensor) => (int)bc.Lightness(sensor);
int ultra(byte sensor) => (int)bc.Distance(sensor);
float eixo_x() => bc.Compass();
float eixo_y() => bc.Inclination();
float angulo_atuador() => bc.AngleActuator();
float angulo_giro_atuador() => bc.AngleScoop();
void delay(int milissegundos) => bc.Wait(milissegundos);

// Dicionário de notas para melhorar o comando de som
Dictionary<string, float> notas = new Dictionary<string, float>(){
    {"C", 16.35f},
    {"C# ", 17.32f},
    {"D", 18.35f},
    {"D#", 19.45f},
    {"E", 20.60f},
    {"F", 21.83f},
    {"F#", 23.12f},
    {"G", 24.50f},
    {"G#", 25.96f},
    {"A", 27.50f},
    {"A#", 29.14f},
    {"B", 30.87f},
    {"C1", 32.70f},
    {"C#", 34.65f},
    {"MUDO", 0},
    {"", 0}
};

void som(string nota, int tempo) => bc.PlaySoundHertz(1, notas[nota], tempo, "QUADRADA");
void led(byte R, byte G, byte B) => bc.TurnLedOn(R, G, B);
void print(int linha, object texto) => bc.PrintConsole(linha, texto.ToString());
void limpar_console() => bc.ClearConsole();
void limpar_linha(int linha) => bc.ClearConsoleLine(linha);

bool tem_linha(int sensor) => (bc.returnBlue(sensor) < 24);

bool azul(int sensor)
{
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 31, media_verde = 40, media_azul = 35;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((vermelho < media_vermelho) && (verde < media_verde) && (azul > media_azul));
}

bool verde(int sensor)
{
    float val_vermelho = bc.ReturnRed(sensor);
    float val_verde = bc.ReturnGreen(sensor);
    float val_azul = bc.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96) || cor(sensor) == "VERDE");
}

bool preto(int sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((bc.lightness(sensor) < media_meio) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((bc.lightness(sensor) < media_fora) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    return false;
}

bool branco(int sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((bc.lightness(sensor) > media_meio) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((bc.lightness(sensor) > media_fora) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    return false;
}

void calibrar()
{
    ajustar_linha();
    media_meio = (luz(1) + luz(2)) / 4.2f;
    media_fora = (luz(0) + luz(3)) / 4.2f;

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

    print(3, $"calibração: {media_meio}");
}

void verifica_calibrar()
{
    if (proximo(eixo_x(), saida1))
    {
        calibrar();
    }

    else if (proximo(eixo_x(), saida2))
    {
        calibrar();
    }
}

void ler_cor()
{
    preto0 = preto(0);
    preto1 = preto(1);
    preto2 = preto(2);
    preto3 = preto(3);

    verde0 = verde(0);
    verde1 = verde(1);
    verde2 = verde(2);
    verde3 = verde(3);

    preto_curva_dir = ((tem_linha(0) || cor(0) == "PRETO" || preto(0)));
    preto_curva_esq = ((tem_linha(3) || cor(3) == "PRETO" || preto(3)));
}

bool angulo_reto()
{
    foreach (int angulo_verificado in angulos_retos)
    {
        if (proximo(eixo_x(), angulo_verificado))
        {
            return true;
        }
    }
    return false;
}
// Métodos de movimentação e outros

void mover(int esquerda, int direita) => bc.MoveFrontal(direita, esquerda);
void rotacionar(int velocidade, int graus) => bc.MoveFrontalAngles(velocidade, graus);
void encoder(int velocidade, float rotacoes) => bc.MoveFrontalRotations(velocidade, rotacoes);
void parar() { bc.MoveFrontal(0, 0); delay(10); }
void travar() { bc.MoveFrontal(0, 0); delay(999999999); }

// Curva para a esquerda em graus
void girar_esquerda(int graus)
{
    float objetivo = converter_graus(eixo_x() - graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Curva para a direita em graus
void girar_direita(int graus)
{
    float objetivo = converter_graus(eixo_x() + graus);

    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Gira para a esquerda até um objetivo (bússola)
void objetivo_esquerda(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(int objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(1000, -1000);
    }
    parar();
}

// Alinha o robô no ângulo reto mais próximo
void alinhar_angulo()
{
    led(255, 255, 0);
    print(2, "Alinhando robô");

    int alinhamento = 0;
    float angulo = eixo_x();

    if (angulo_reto())
    {
        return;
    }

    if ((angulo > 315) || (angulo <= 45))
    {
        alinhamento = 0;
    }
    else if ((angulo > 45) && (angulo <= 135))
    {
        alinhamento = 90;
    }
    else if ((angulo > 135) && (angulo <= 225))
    {
        alinhamento = 180;
    }
    else if ((angulo > 225) && (angulo <= 315))
    {
        alinhamento = 270;
    }

    angulo = eixo_x();

    if ((alinhamento == 0) && (angulo > 180))
    {
        objetivo_direita(alinhamento);
    }
    else if ((alinhamento == 0) && (angulo < 180))
    {
        objetivo_esquerda(alinhamento);
    }
    else if (angulo < alinhamento)
    {
        objetivo_direita(alinhamento);
    }
    else if (angulo > alinhamento)
    {
        objetivo_esquerda(alinhamento);
    }

    limpar_linha(2);
    bc.TurnLedOff();
}

// Ajusta os sensores na linha preta
void ajustar_linha()
{
    led(255, 255, 0);


    tempo_correcao = millis() + 150;
    while (cor(0) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(-1000, 1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(1) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(-1000, 1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(3) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(1000, -1000);
    }
    tempo_correcao = millis() + 150;
    while (cor(2) == "PRETO" && millis() < tempo_correcao)
    {
        bc.onTF(1000, -1000);
    }

    parar();
    bc.TurnLedOff();
}
// Segue as linhas
void seguir_linha()
{
    print(1, $"Seguindo linha: {velocidade}");
    bc.TurnLedOff();
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        tempo_correcao = millis() + 210;
        while (millis() < tempo_correcao)
        {
            mover(1000, -1000);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                return;
            }
        }
        mover(-1000, 1000);
        delay(210);
        parar();

        // Confirma que está perdido
        print(1, "Perdi a linha...");
        led(255, 0, 0);
        som("F#", 64);
        som("", 16);
        som("F#", 64);
        // Vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() + 1750;
        while (millis() < tras)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                break;
            }
        }
        delay(150);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }

    // Está saindo da pista (detectou o azul do fim da arena)
    if (azul(1) || azul(2))
    {
        print(1, "Saí da arena...");
        led(255, 0, 0);
        som("B", 64);
        som("MUDO", 16);
        som("B", 64);
        // Calcula a diferença desde a última correção e vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() - ultima_correcao + 150;
        tempo_correcao = millis() + tras;
        while (millis() < tempo_correcao)
        {
            mover(-velocidade, -velocidade);
            if (cor(0) == "PRETO" || cor(1) == "PRETO" || cor(2) == "PRETO" || cor(3) == "PRETO")
            {
                break;
            }
        }
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
    }


    // Incremento da velocidade de acordo com o tempo
    if ((millis() > update_time) && (velocidade < velocidade_max))
    {
        update_time = millis() + 32;
        velocidade++;
    }

    // Área do seguidor===============================================================================

    // Se viu preto no sensor da direita
    if (preto1)
    {
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;


        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(1) || preto(2))
            {
                break;
            }
            mover(1000, -1000);
        }
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    // Se viu preto no sensor da direita
    else if (preto2)
    {
        // Atualiza a velocidade para o padrão
        velocidade = velocidade_padrao;

        // Inicia a correção e gira até encontrar a linha novamente ou estourar o tempo
        tempo_correcao = millis() + 210;
        while (tempo_correcao > millis())
        {
            if (branco(2) || preto(1))
            {
                break;
            }
            mover(-1000, 1000);
        }
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        mover(velocidade, velocidade);
    }
}
// Verificação do beco sem saída
bool beco()
{
    // Para, lê as cores e verifica se está na condição do beco
    parar();
    delay(64);
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        ajustar_linha();
        delay(64);
        ler_cor();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            // Feedback visual e sonoro para indicar que entrou na condição
            print(1, "BECO SEM SAÍDA");
            led(0, 255, 0);
            som("F#", 100);
            som("D", 100);
            som("F#", 100);
            som("D", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        return false;
    }
    return false;
}

// Verificação das condições de verde
bool verifica_verde()
{
    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "CURVA VERDE - Direita");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F", 100);
            while (!(tem_linha(1)))
            {
                mover(190, 190);
            }
            som("G", 100);
            while (cor(1) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("A", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_direita(20);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }

    // Verifica os sensores da direita
    else if (verde2 || verde3)
    {
        // Verificação do beco sem saída
        if (beco()) { return true; }
        // Se alinha na linha atrás e verifica novamente
        print(1, "CURVA VERDE - Esquerda");
        encoder(-300, 2);
        ajustar_linha();
        encoder(300, 2);
        delay(64);
        ler_cor();
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            led(0, 255, 0);
            som("F", 100);
            while (!(tem_linha(2)))
            {
                mover(190, 190);
            }
            som("G", 100);
            while (cor(2) == "PRETO")
            {
                mover(190, 190);
            }
            parar();
            som("A", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo reto
            encoder(300, 10);
            girar_esquerda(20);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    break;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            ajustar_linha();
            encoder(-300, 2);
            ajustar_linha();
            velocidade = velocidade_padrao;
            ultima_correcao = millis();
            calibrar();
            return true;
        }
        // Tratamento de falsos positivos
        else
        {
            return false;
        }
    }
    else
    {
        return false;
    }
}


bool verifica_curva()
{
    ler_cor();
    if (verifica_verde()) { return true; }

    else if (preto_curva_dir)
    {
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Direita");
        led(0, 0, 0);
        som("C", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !azul(1) && !azul(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) && !azul(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                encoder(-300, 7f);
                mover(-1000, 1000);
                delay(300);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(1000, -1000);
        }
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        if (verifica_verde()) { return true; }
        encoder(-300, 1.5f);
        if (verifica_verde()) { return true; }
        print(1, "CURVA PRETO - Esquerda");
        led(0, 0, 0);
        som("C", 100);
        encoder(300, 7.5f);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !azul(1) && !azul(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) && !azul(2))
        {
            ler_cor();
            if (proximo(eixo_x(), objetivo))
            {
                encoder(-300, 7f);
                mover(1000, -1000);
                delay(300);
                alinhar_angulo();
                encoder(300, 2f);
                ajustar_linha();
                velocidade = velocidade_padrao;
                ultima_correcao = millis();
                calibrar();
                return true;
            }
            mover(-1000, 1000);
        }
        delay(200);
        ajustar_linha();
        encoder(-300, 2);
        ajustar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }
    else
    {
        return false;
    }
}

// Variável de controle para ligar/desligar o debug
bool debug = true;

// Método principal
void Main()
{
    if (!debug)
    {
        calibrar();
        ultima_correcao = millis();
    }
    // Loop principal do programa
    while (!debug)
    {
        verifica_calibrar();
        seguir_linha();
        verifica_curva();
    }

    // Loop para debug
    while (debug)
    {
        print(1, "Debug!!");
    }
}
