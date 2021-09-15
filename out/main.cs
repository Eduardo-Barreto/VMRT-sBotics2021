// Declaração das variáveis principais de todo o projeto, separadas por tipos
byte velocidade = 180,
        media_meio = 0,
        direcao_triangulo = 0,
        direcao_saida = 0,
        lugar = 0;

const byte velocidade_padrao = 185,
        velocidade_max = 220,
        limite_branco = 55;

float saida1 = 0,
        saida2 = 0,
        ultra_frente = 0,
        ultra_direita = 0,
        ultra_esquerda = 0,
        direcao_inicial = 0; // variavel para a posição inical do robô no resgate

int tempo_correcao = 0,
        update_time = 16,
        ultima_correcao = 0,
        update_obstaculo = 0,
        update_rampa = 0,
        update_curva = 0,
        timeout = 0,
        init_time = 0;

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

short[] angulos_retos = { 0, 90, 180, 270 };

char lado_ajuste = '0';
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
    return (graus % 360 + 360) % 360;
}

void erro(object texto)
{
    throw new Exception(texto.ToString());
}

string[] rainbow = { "#f90300", "#f89621", "#fce91f", "#42b253", "#2aaae1", "#0047ab", "#9400d3" };

void rainbow_console(string word, string[] colors, int time = 5000)
{

    string word_final = "";
    int colors_index = 0;

    string colorize(char texto, string cor) => $"<color={cor}>{texto}</color>";

    bot.ResetTimer();
    while (bot.Timer() < time)
    {
        colors_index++;

        word_final = "";
        for (byte i = 0; i < word.Length; i++)
        {
            word_final += colorize(word[i], colors[colors_index % colors.Length]);
            bot.TurnLedOn(colors[colors_index % colors.Length]);
            colors_index++;
        }

        bot.Print($"<b><size=60><align=center>{word_final}</align></size></b>\n");
        bot.Wait(200);
    }

}
// Métodos de leitura e outros

int millis() => (int)(bot.Timer());
bool toque() => (bot.Touch(0));
string cor(byte sensor) => bot.ReturnColor(sensor);
byte luz(byte sensor) => (byte)bot.Lightness(sensor);
short ultra(byte sensor) => (short)bot.Distance(sensor);
short eixo_x() => (short)(bot.Compass());
short eixo_y() => (short)(bot.Inclination());
float angulo_atuador() => bot.AngleActuator();
float angulo_giro_atuador() => bot.AngleScoop();
bool tem_vitima() => bot.HasVictim();
void delay(int milissegundos) => bot.Wait(milissegundos);
float forca_motor() => bot.RobotSpeed();

void som(string nota, short tempo) => bot.PlayNote(0, nota, tempo);

Dictionary<string, string> cores = new Dictionary<string, string>(){
    {"preto", "#000000"},
    {"cinza escuro", "#333332"},
    {"cinza claro", "#656565"},
    {"branco", "#fffffe"},
    {"amarelo", "#ffcc02"},
    {"verde", "#009245"},
    {"vermelho", "#ff3232"},
    {"azul", "#28ade2"}
};
void led(byte R, byte G, byte B) => bot.TurnLedOn(R, G, B);
void led(string cor)
{
    if (cor == "desligado")
    {
        bot.TurnLedOff();
        return;
    }
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    bot.TurnLedOn(cor);
}

void console_led(byte linha, object texto, string cor, bool ligar_led = true)
{
    if (!cor.StartsWith("#"))
    {
        cor = cores[cor];
    }
    string texto_aux = texto.ToString();
    texto_aux = texto_aux.Replace("<:", $"<color={cor}>");
    texto_aux = texto_aux.Replace(":>", "</color>");
    print(linha, "<align=center>" + texto_aux.ToString() + "</align>");
    if (!ligar_led)
    {
        bot.TurnLedOff();
        return;
    }
    bot.TurnLedOn(cor);
}


void print(byte linha, object texto, string align = "center") { if (console) bot.Print(linha - 1, $"<align={align}>" + texto.ToString()); }

void limpar_console() => bot.ClearConsole();
void limpar_linha(byte linha) => print((byte)(linha - 1), " ");

bool tem_linha(byte sensor) => (bot.returnRed(sensor) < 24);

bool colorido(byte sensor) => (bot.returnRed(sensor) != bot.ReturnBlue(sensor));

bool vermelho(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 66, media_verde = 16, media_azul = 16;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERMELHO");
}

/* bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 20, media_verde = 65, media_azul = 14;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    print(1, $"{vermelho} | {verde} | {azul}");
    return ((vermelho < media_vermelho) && (verde > media_verde) && (azul < media_azul) && (verde < 96) || cor(sensor) == "VERDE");
} */

bool verde(byte sensor)
{
    float val_vermelho = bot.ReturnRed(sensor);
    float val_verde = bot.ReturnGreen(sensor);
    float val_azul = bot.ReturnBlue(sensor);
    byte media_vermelho = 13, media_verde = 82, media_azul = 4;
    int RGB = (int)(val_vermelho + val_verde + val_azul);
    sbyte vermelho = (sbyte)(map(val_vermelho, 0, RGB, 0, 100));
    sbyte verde = (sbyte)(map(val_verde, 0, RGB, 0, 100));
    sbyte azul = (sbyte)(map(val_azul, 0, RGB, 0, 100));
    return ((proximo(vermelho, media_vermelho, 2) && proximo(verde, media_verde, 2) && proximo(azul, media_azul, 2)) || cor(sensor) == "VERDE");
}

bool preto(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) < media_meio) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) < limite_branco) || (cor(sensor) == "PRETO") || tem_linha(sensor))
        {
            return true;
        }
    }
    return false;
}

bool branco(byte sensor)
{
    if (sensor == 1 || sensor == 2)
    {
        if ((luz(sensor) > media_meio) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    if (sensor == 0 || sensor == 3)
    {
        if ((luz(sensor) > limite_branco) || (cor(sensor) == "BRANCO"))
        {
            return true;
        }
    }
    return false;
}

void calibrar()
{
    alinhar_linha(true);
    media_meio = (byte)Math.Round((luz(1) + luz(2)) / 3f);

    saida1 = converter_graus(eixo_x() + 90);
    saida2 = converter_graus(eixo_x() - 90);

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

    preto_curva_dir = preto(0);
    preto_curva_esq = preto(3);
}

void ler_ultra()
{
    ultra_frente = ultra(0);
    ultra_direita = ultra(1);
    ultra_esquerda = ultra(2);
}

bool angulo_reto()
{
    foreach (short angulo_verificado in angulos_retos)
    {
        if (proximo(eixo_x(), angulo_verificado))
        {
            return true;
        }
    }
    return false;
}

string luz_marker(int luz)
{
    string hex_str = Convert.ToString(luz, 16);
    string grayscaleHex = hex_str.PadLeft(2, '0');
    string marker = "#" + grayscaleHex + grayscaleHex + grayscaleHex;
    return "<" + "mark=" + marker + ">---" + "<" + "/" + "mark>";
}

void print_luz_marker()
{
    string luz0 = (luz(0).ToString()).PadLeft(3, '0');
    string luz1 = (luz(1).ToString()).PadLeft(3, '0');
    string luz2 = (luz(2).ToString()).PadLeft(3, '0');
    string luz3 = (luz(3).ToString()).PadLeft(3, '0');
    string marker0 = (verde0) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(0)));
    string marker1 = (verde1) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(1)));
    string marker2 = (verde2) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(2)));
    string marker3 = (verde3) ? ("<mark=#009245>---</mark>") : (luz_marker(luz(3)));
    print(2, $"{luz3} | {luz2} | {luz1} | {luz0}");
    print(3, $"{marker3} | {marker2} | {marker1} | {marker0}");
    print(4, "");
}
// Métodos de movimentação e outros

void abrir_atuador() => bot.OpenActuator();
void fechar_atuador() => bot.CloseActuator();
void mover(int esquerda, int direita) => bot.MoveFrontal(direita, esquerda);
void encoder(int velocidade, float rotacoes) => bot.MoveFrontalRotations(velocidade, rotacoes);
void parar(int tempo = 10) { bot.MoveFrontal(0, 0); delay(tempo); }
void travar() { bot.MoveFrontal(0, 0); delay(999999999); }

void mover_tempo(int velocidade, int tempo)
{
    timeout = bot.Timer() + tempo;
    while (bot.Timer() < timeout)
    {
        if (velocidade < 0 && toque())
        {
            break;
        }
        mover(velocidade, velocidade);
    }
    parar();
}

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
void objetivo_esquerda(float objetivo)
{
    while (!proximo(eixo_x(), objetivo))
    {
        mover(-1000, 1000);
    }
    parar();
}

// Gira para a direita até um objetivo (bússola)
void objetivo_direita(float objetivo)
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
    led("amarelo");

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

    led("desligado");
}

// Ajusta os sensores na linha preta
void alinhar_linha(bool por_luz = false)
{
    led("amarelo");

    if (por_luz)
    {
        if (luz(0) < 30 && luz(1) < 30 && luz(2) < 30 && luz(3) < 30)
        {
            mover(200, 200);
            delay(200);
            return;
        }
        tempo_correcao = millis() + 150;
        while (luz(0) < 30 && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(1) < 30 && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (luz(3) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (luz(2) < 30 && millis() < tempo_correcao)
            mover(-1000, 1000);
    }
    else
    {
        tempo_correcao = millis() + 150;
        while (cor(0) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(1) == "PRETO" && millis() < tempo_correcao)
            mover(1000, -1000);
        tempo_correcao = millis() + 150;
        while (cor(3) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
        tempo_correcao = millis() + 150;
        while (cor(2) == "PRETO" && millis() < tempo_correcao)
            mover(-1000, 1000);
    }

    delay(64);
    parar();
    led("desligado");
}

void levantar_atuador()
{
    // Levanta o atuador para o ângulo correto
    bot.ActuatorSpeed(150);
    bot.ActuatorUp(100);
    if (angulo_atuador() >= 0 && angulo_atuador() < 88)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorUp(600);
    }
}

void abaixar_atuador()
{
    if (angulo_atuador() > 5)
    {
        bot.ActuatorSpeed(150);
        bot.ActuatorDown(600);
    }
}
bool verifica_saida()
{
    if (lugar == 3)
    {
        if ((vermelho(0)) || (vermelho(1)) || (vermelho(2)) || (vermelho(3)))
        {
            alinhar_angulo();
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 144);
            som("MUDO", 15);
            som("D3", 144);
            som("MUDO", 15);
            som("C3", 175);
            som("MUDO", 200);

            som("A#2", 112);
            som("A2", 112);
            som("G2", 112);
            som("MUDO", 15);
            som("F2", 300);
            som("MUDO", 150);
            som("F3", 300);
            encoder(300, 15);
            rainbow_console("ARENA FINALIZADA", rainbow);
            travar();
            return true;
        }
        return false;
    }
    // Está saindo da pista (detectou o fim da arena)
    else if (vermelho(1) || vermelho(2))
    {
        console_led(1, "<:Saí da arena...:>", "vermelho");
        som("B3", 64);
        som("MUDO", 16);
        som("B3", 64);
        // Calcula a diferença desde a última correção e vai para trás até encontrar uma linha ou estourar o tempo
        mover(-velocidade, -velocidade);
        delay(150);
        int tras = millis() - ultima_correcao;
        tempo_correcao = millis() + tras;
        while (millis() < tempo_correcao)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                if (!vermelho(1) && !vermelho(2))
                {
                    delay(200);
                    break;
                }
            }
        }
        alinhar_linha();
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        return true;
    }
    else
    {
        return false;
    }
}

// Segue as linhas
void seguir_linha()
{
    if (verifica_saida()) { return; }
    if (verifica_curva()) { return; }
    console_led(1, $"Seguindo linha: <:{velocidade}:>", "azul", false);
    ler_cor();

    // Área de ajustes===============================================================================

    // Perdeu a linha (muito tempo sem se corrigir)
    if ((millis() - ultima_correcao) > 1500)
    {
        // Se tem linha na posição atual, retorna ao normal
        if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
        {
            alinhar_linha();
            ultima_correcao = millis();
            return;
        }

        // Começa a verificar se há linha por perto
        float objetivo = (lado_ajuste == 'd') ? (converter_graus(eixo_x() + 10)) : (converter_graus(eixo_x() - 10));
        while (!proximo(eixo_x(), objetivo))
        {
            if (lado_ajuste == 'd')
                mover(1000, -1000);
            else
                mover(-1000, 1000);

            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                alinhar_linha(true);
                alinhar_linha();
                ultima_correcao = millis();
                return;
            }
        }
        if (lado_ajuste == 'd')
            girar_esquerda(10);
        else
            girar_direita(10);

        parar();

        // Confirma que está perdido
        console_led(1, "<:Perdi a linha...:>", "vermelho");
        som("F#3", 64);
        som("MUDO", 16);
        som("F#3", 64);
        // Vai para trás até encontrar uma linha ou estourar o tempo
        int tras = millis() + 1750;
        while (millis() < tras)
        {
            mover(-velocidade, -velocidade);
            if (tem_linha(0) || tem_linha(1) || tem_linha(2) || tem_linha(3))
            {
                break;
            }
            if (toque())
            {
                parar();
                break;
            }
        }
        delay(150);
        alinhar_linha(true);
        alinhar_linha();
        velocidade = (byte)(velocidade - 15);
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
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
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
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = 'd';
    }

    // Se viu preto no sensor da direita
    else if (preto2)
    {
        if (verifica_curva()) { return; }
        if (verifica_saida()) { return; }
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
        verifica_curva();
        // Vai para a frente por um pequeno tempo e atualiza a última correção
        mover(velocidade, velocidade);
        delay(5);
        ultima_correcao = millis();
        lado_ajuste = 'e';
    }

    // Se está certo na linha só vai para frente com a velocidade atual
    else
    {
        verifica_curva();
        mover(velocidade, velocidade);
    }
}
bool falso_verde()
{
    /*
    Falso Verde: Verifica se o robô realmente está no verde e não passou reto de uma outra encruzilhada
        Vem da verificação do verde
        Define um tempo máximo de verificação de 200 milissegundos
        Enquanto está nesse tempo:
            Anda para trás
            Se encontrar a cor preta, vai para frente e retorna verdadeiro (era realmente um falso verde)
            Senão, continua a movimentação, retorna falso (falso falso verde = verde verdadeiro), e realiza a curva
    */
    parar();
    mover_tempo(-180, 63);
    int tempo_check_preto = millis() + 127;
    while (millis() < tempo_check_preto)
    {
        mover(-180, -180);
        if (cor(0) == "PRETO" || cor(3) == "PRETO")
        {
            mover_tempo(300, 288);
            return true;
        }
    }
    mover(200, 200);
    delay(196);
    parar();
    return false;
}

bool beco()
{
    /*
    Beco: Verifica se o robô está no Beco sem saída (verde dos dois lados da encruzilhada)
        Para o robô por tempo suficiente para atualizar a leitura dos sensores
        Se detectar verde dos dois lados:
            Ajusta na linha e para novamente por tempo suficiente para atualizar a leitura dos sensores
            Se novamente identificar verde dos dois lados
                Verifica se é realmente uma encruzilhada (falso_verde())
                Confirmado o beco, acende o led verde e escreve no console
                Indica pelo som que caiu na condição correta
                Vai para frente e faz uma curva de 170 graus para a direita
                Gira até encontrar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Para, lê as cores e verifica se está na condição do beco
    parar();
    ler_cor();
    if ((verde0 || verde1) && (verde2 || verde3))
    {
        // Ajusta na linha, para e confirma a leitura
        print_luz_marker();
        alinhar_linha();
        delay(64);
        ler_cor();
        print_luz_marker();
        if ((verde0 || verde1) && (verde2 || verde3))
        {
            print_luz_marker();
            if (falso_verde()) { return false; }
            // Feedback visual e sonoro para indicar que entrou na condição
            console_led(1, "<:<b>BECO SEM SAÍDA</b>:>", "verde");
            som("F#3", 100);
            som("D3", 100);
            som("F#3", 100);
            som("D3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            encoder(300, 12);
            girar_direita(170);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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
    /*
    Verifica Verde: Verifica se o robô está em uma encruzilhada com verde
        Atualiza as leituras dos sensores
        Se encontrar verde em algum dos sensores da direita
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à direita
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar verde em algum dos sensores da esquerda
            Verifica se é um beco
            Se não for, ajusta na linha, atualiza os sensores e ajusta novamente
            Atualiza os sensores mais uma vez e verifica novamente se o verde está ali
                Caso esteja, verifica novamente pelo beco
                Se não for beco, verifica se não pulou uma encruzilhada reta (falso_verde())
                Confirmando que está certo, dá o feedback visual e sonoro
                Vai para frente e inicia a curva com 60 graus à esquerda
                Gira até achar a linha ou um ângulo ortogonal
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza os valores de cor e verifica os sensores da direita
    ler_cor();
    if (verde0 || verde1)
    {
        parar();
        alinhar_linha();
        print_luz_marker();
        if (beco()) { return true; }
        if (verde0 || verde1)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Direita", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 431);
            girar_direita(30);
            while (!tem_linha(1))
            {
                mover(1000, -1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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

    // Verifica os sensores da esquerda
    else if (verde2 || verde3)
    {
        parar();
        alinhar_linha();
        print_luz_marker();
        // Verificação do beco sem saída
        if (beco()) { return true; }
        if (verde2 || verde3)
        {
            // Nova verificação do beco
            if (beco()) { return true; }
            // Feedback visual e sonoro para indicar que entrou na condição e se alinhou
            console_led(1, "<:<b>CURVA VERDE</b>:> - Esquerda", "verde");
            som("F3", 100);
            som("G3", 100);
            som("A3", 100);
            // Vai para frente e realiza a curva, girando até encontrar a linha ou um ângulo ortogonal
            mover_tempo(300, 431);
            girar_esquerda(30);
            while (!tem_linha(2))
            {
                mover(-1000, 1000);
                if (angulo_reto())
                {
                    encoder(-300, 2);
                    velocidade = (byte)(velocidade_padrao - 5);
                    ultima_correcao = millis();
                    calibrar();
                    return true;
                }
            }
            // Se ajusta na linha e atualiza os valores de correção e velocidade
            delay(200);
            alinhar_linha();
            encoder(-300, 2);
            alinhar_linha();
            velocidade = (byte)(velocidade_padrao - 5);
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

// Verificações de curvas no preto
bool verifica_curva()
{
    /*
    Verifica Curva: Verifica se o robô está em alguma curva de 90° no preto
        Atualiza as leituras dos sensores
        Verifica se está no verde ou no fim da arena
        Se encontrar preto no sensor da direita
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
            Atualiza a leitura dos sensores
            Se estiver no vermelho (fim da arena), retorna falso (não é curva)
            Se encontrar preto no sensor da esquerda (encruz reta)
                Vai para frente e retorna falso (não é curva)
            Verifica novamente pela saída da arena (vermelho) e verde
            Vai para trás e verifica novamente pelo verde
            Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
            Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
            Com a curva totalmente confirmada, continua girando até achar a linha
            Se passar de 115 graus, asssume que é o ladrilho de Curva C com GAP
                Se alinha atrás e por graus
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
            Se alinha na linha
            Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
        Se encontrar preto no sensor da esquerda
            Para o robô por tempo suficiente para atualizar a leitura dos sensores
                Atualiza a leitura dos sensores
                Se estiver no vermelho (fim da arena), retorna falso (não é curva)
                Se encontrar preto no sensor da direita (encruz reta)
                    Vai para frente e retorna falso (não é curva)
                Verifica novamente pela saída da arena (vermelho) e verde
                Vai para trás e verifica novamente pelo verde
                Confirmando que é uma corva normal, dá os feedbacks visuais e sonoros
                Vai para frente e inicia com uma curva de 15 graus, verificando se há linha na frente (encruz. reta)
                Com a curva totalmente confirmada, continua girando até achar a linha
                Se passar de 115 graus, assume que é o ladrilho de Curva C com GAP
                    Se alinha atrás e por graus
                    Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
                Se alinha na linha
                Finaliza ajustando na linha e atualiza os valores de velocidade, última correção e calibração
    */

    // Atualiza leituras de cores, verifica se está no verde e depois no vermelho

    if (millis() < update_curva) { return false; }

    ler_cor();
    if (verifica_verde()) { return true; }
    if (verifica_saida()) { return false; }

    else if (preto_curva_dir)
    {
        ler_cor();
        print_luz_marker();
        if (vermelho(0) || colorido(0))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_esq)
        {
            if (verifica_verde()) { return false; }
            // Verifica o verde mais uma vez, vai para trás e verifica novamente
            timeout = millis() + 143;
            while (millis() < timeout)
            {
                mover(-300, -300);
                if (verifica_verde()) { return true; }
            }
            parar();
            mover_tempo(300, 431);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return false; }
        // Verifica o verde mais uma vez, vai para trás e verifica novamente
        timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        // Feedbacks visuais e sonoross de que entrou na condição da curva
        console_led(1, "<b>CURVA PRETO</b> - Direita", "preto");
        som("C3", 100);
        // Vai para frente e começa a verificar se não existe uma linha reta na frente
        mover_tempo(300, 351);
        float objetivo = converter_graus(eixo_x() + 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(1000, -1000);
        }
        // Confirmada a curva, gira até encontrar uma linha ou passar do ângulo máximo
        objetivo = converter_graus(eixo_x() + 115);
        while (!tem_linha(1) || vermelho(1))
        {
            mover(1000, -1000);
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 223);
                objetivo = converter_graus(eixo_x() - 115);
                while (!tem_linha(2) || vermelho(2))
                {
                    mover(-1000, 1000);
                    if (proximo(eixo_x(), objetivo))
                    {
                        mover_tempo(-300, 111);
                        while (!tem_linha(1) || vermelho(1))
                        {
                            mover(1000, -1000);
                        }
                        delay(159);
                        alinhar_angulo();
                        parar();
                        mover_tempo(300, 181);
                        velocidade = (byte)(velocidade_padrao - 5);
                        ultima_correcao = millis();
                        calibrar();
                        return true;
                    }
                }
                delay(150);
                parar();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
        }
        // Se ajusta na linha e atualiza os valores de correção e velocidade
        delay(200);
        alinhar_linha();
        alinhar_linha(true);
        velocidade = velocidade_padrao;
        ultima_correcao = millis();
        calibrar();
        return true;
    }

    else if (preto_curva_esq)
    {
        parar(64);
        ler_cor();
        print_luz_marker();
        if (vermelho(3) || colorido(3))
        {
            update_curva = millis() + 159;
            return false;
        }
        if (preto_curva_dir)
        {
            if (verifica_verde()) { return false; }
            // Verifica o verde mais uma vez, vai para trás e verifica novamente
            timeout = millis() + 143;
            while (millis() < timeout)
            {
                mover(-300, -300);
                if (verifica_verde()) { return true; }
            }
            parar();
            mover_tempo(300, 431);
            return false;
        }
        if (verifica_saida()) { return false; }
        if (verifica_verde()) { return false; }
        timeout = millis() + 143;
        while (millis() < timeout)
        {
            mover(-300, -300);
            if (verifica_verde()) { return true; }
        }
        parar();
        console_led(1, "<b>CURVA PRETO</b> - Esquerda", "preto");
        som("C3", 100);
        mover_tempo(300, 351);
        float objetivo = converter_graus(eixo_x() - 15);
        while (!proximo(eixo_x(), objetivo))
        {
            if ((tem_linha(1) || tem_linha(2)) && !vermelho(1) && !vermelho(2))
            {
                return false;
            }
            mover(-1000, 1000);
        }
        objetivo = converter_graus(eixo_x() - 115);
        while (!tem_linha(2) || vermelho(2))
        {
            mover(-1000, 1000);
            if (proximo(eixo_x(), objetivo))
            {
                mover_tempo(-300, 223);
                objetivo = converter_graus(eixo_x() + 115);
                while (!tem_linha(1) || vermelho(1))
                {
                    mover(1000, -1000);
                    if (proximo(eixo_x(), objetivo))
                    {
                        mover_tempo(-300, 111);
                        while (!tem_linha(2) || vermelho(2))
                        {
                            mover(-1000, 1000);
                        }
                        delay(159);
                        alinhar_angulo();
                        parar();
                        mover_tempo(300, 181);
                        velocidade = (byte)(velocidade_padrao - 5);
                        ultima_correcao = millis();
                        calibrar();
                        return true;
                    }
                }
                delay(150);
                parar();
                mover_tempo(300, 181);
                velocidade = (byte)(velocidade_padrao - 5);
                ultima_correcao = millis();
                calibrar();
                return true;
            }
        }
        delay(200);
        alinhar_linha();
        alinhar_linha(true);
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
bool verifica_obstaculo(bool contar_update = true)
{
    if (contar_update && millis() < update_obstaculo) { return false; }
    if (ultra(0) < 35)
    {
        parar();
        if (angulo_atuador() >= 0 && angulo_atuador() < 88)
            mover_tempo(-200, 79);
        levantar_atuador();
        console_led(1, "<:POSSÍVEL OBSTÁCULO:>", "azul");
        timeout = millis() + 1500;
        while (ultra(0) > 12)
        {
            ultima_correcao = millis();
            seguir_linha();
            if (ultra(0) > 20 && millis() > timeout)
            {
                console_led(1, "<:OBSTÁCULO FALSO:>", "vermelho");
                parar();
                abaixar_atuador();
                return false;
            }
        }
        parar();
        console_led(1, "<:OBSTÁCULO CONFIRMADO:>", "azul");
        alinhar_angulo();
        alinhar_ultra(12);
        parar();
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        som("MUDO", 16);
        som("E3", 64);
        girar_direita(45);
        som("E3", 32);
        mover_tempo(300, 543);
        som("E3", 32);
        girar_esquerda(15);
        mover_tempo(300, 239);
        girar_esquerda(15);
        mover_tempo(300, 239);
        girar_esquerda(15);
        som("E3", 32);
        // 495
        timeout = millis() + 559;
        while (ultra(2) > 15)
        {
            if (millis() > timeout) { break; }
            mover(300, 300);
        }
        while (ultra(2) < 15)
        {
            if (millis() > timeout) { break; }
            mover(300, 300);
        }
        mover_tempo(300, 127);//alsuagfalgbasjpiasdfjkadfsajkl todo
        som("E3", 32);
        girar_esquerda(60);
        som("E3", 32);
        timeout = millis() + 495;
        while (millis() < timeout)
        {
            if (preto(0) || preto(1))
            {
                break;
            }
            mover(200, 200);
        }
        parar();
        som("D3", 32);
        mover_tempo(300, 335);
        som("E3", 32);
        float objetivo = converter_graus(eixo_x() + 45);
        while (!preto(1))
        {
            if (proximo(eixo_x(), objetivo))
            {
                break;
            }
            mover(1000, -1000);
        }
        delay(200);
        alinhar_angulo();
        tempo_correcao = millis() + 350;
        while (millis() < tempo_correcao)
        {
            if (toque())
            {
                break;
            }
            mover(-150, -150);
        }
        parar();
        som("D3", 32);
        som("MUDO", 16);
        som("D3", 32);
        alinhar_linha();
        abaixar_atuador();
        if (proximo(eixo_y(), 350, 3))
            levantar_atuador();
        update_obstaculo = millis() + 100;
        return true;
    }
    return false;
}
bool verifica_gangorra()
{
    /*
    Verifica gangorra: Verifica se o robô está numa gangorra
        Vindo do verifica_rampa, se estiver num ângulo próximo a 0, com 5 de tolerancia
            Alinha no ângulo ortogonal mais próximo
            Escreve no console que está na gangorra e acende o led vermelho
            Vai um pouquinho pra trás e espera 600 milissegundos
            Alinha no ângulo ortogonal mais próximo novamente
            retorna verdadeiro
    */

    if (eixo_y() > 350 || eixo_y() < 5)
    {
        alinhar_angulo();
        parar();
        console_led(1, "<:GANGORRA:>", "vermelho");
        encoder(-300, 2);
        delay(600);
        alinhar_angulo();
        return true;
    }
    return false;
}

bool verifica_rampa()
{
    /* 
    Verifica rampa: Verifica se o robô está numa rampa
        Quando a inclinação for próxima de 350
            Levanta o atuador
            Define um tempo para chegar ao topo da rampa
            Inicia a subida
                Segue linha
                Verifica se é uma gangorra
            Abaixa o atuador e retorna

    */
    if (millis() < update_rampa)
        return false;

    if (proximo(eixo_y(), 350))
    {
        parar();
        levantar_atuador();
        int tempo_subir = millis() + 2300;
        bool flag_subiu = false;
        int tempo_check_gangorra = millis() + 400;
        while (millis() < tempo_subir)
        {
            if (millis() > tempo_check_gangorra && proximo(eixo_y(), 340))
            {
                flag_subiu = true;
            }
            if (flag_subiu && verifica_gangorra()) { break; }
            ultima_correcao = millis();
            seguir_linha();
            if (lugar != 3 && verifica_rampa_resgate())
                return true;
        }
        parar();
        if (eixo_y() < 10 || eixo_y() > 40)
        {
            timeout = millis() + 400;
            while (eixo_y() < 350 || eixo_y() > 5)
            {
                ultima_correcao = millis();
                seguir_linha();
                if (verifica_obstaculo(false))
                    break;
                if (millis() > timeout)
                    break;

            }
        }
        parar();
        abaixar_atuador();
        update_rampa = millis() + 2000;
        return true;
    }
    return false;

}

bool verifica_rampa_resgate()
{
    /*
    Verifica rampa resgate: Verifica se o robô está na rampa do resgate
        Se o eixo y (inclinação) estiver próximo de 340 com uma sensibilidade de 10
        e os dois ultrassônicos do lado estiverem tampados (com parede)
            Define o lugar global como a rampa do resgate e retorna
    */

    if ((proximo(eixo_y(), 340, 10)) && (ultra(1) < 40 && ultra(2) < 40))
    {
        lugar = 1;
        return true;
    }
    return false;
}
// metodos de movimentação para a area de resgate
//;
void alinhar_ultra(float distancia, bool empinada = true)
{
    if (ultra(0) > distancia)
    {
        while (ultra(0) > distancia + distancia / 6)
        {
            mover(300, 300);
            if (empinada) { check_subida_frente(); }
        }
        while (ultra(0) > distancia + distancia / 5)
        {
            mover(200, 200);
            if (empinada) { check_subida_frente(); }
        }
        while (ultra(0) > distancia)
        {
            mover(100, 100);
            if (empinada) { check_subida_frente(); }
        }
        while (ultra(0) < distancia)
        {
            mover(-75, -75);
        }
    }
    else
    {
        while (ultra(0) < distancia - distancia / 6)
        {
            mover(-300, -300);
        }
        while (ultra(0) < distancia - distancia / 5)
        {
            mover(-200, -200);
        }
        while (ultra(0) < distancia)
        {
            mover(-100, -100);
        }
        while (ultra(0) > distancia)
        {
            mover(75, 75);
        }
    }
    parar();
}

void entregar_vitima()
{
    abrir_atuador();
    abaixar_atuador();
    int timeout_vitima = millis() + 2000;
    while (tem_vitima())
    {
        if (millis() > timeout_vitima)
        {
            fechar_atuador();
            levantar_atuador();
            if (!tem_vitima()) { return; }
            abrir_atuador();
            abaixar_atuador();
            timeout_vitima = millis() + 2000;
        }
        delay(14);
    }
    delay(350);
    fechar_atuador();
    levantar_atuador();
}

void totozinho(byte vezes = 1)
{ // empurra possiveis bolinhas para frente
    for (byte i = 0; i < vezes; i++)
    {
        mover_tempo(250, 300);
        mover_tempo(-300, 300);
    }
    parar();
}

void preparar_atuador(bool apenas_sem_vitima = false)
{
    if (apenas_sem_vitima)
    {
        if (!tem_vitima())
        {
            totozinho();
            abrir_atuador();
            abaixar_atuador();
        }
    }
    else
    {
        totozinho();
        abrir_atuador();
        abaixar_atuador();
    }
}


void check_subida_frente(bool alinhar = true)
{
    if (eixo_y() > 330 && eixo_y() < 340)
    {
        if (eixo_y() > 330 && eixo_y() < 340)
        {
            mover_tempo(-200, 255);
            if (alinhar) { alinhar_angulo(); }
        }
    }
}


void pegar_vitima()
{
    parar();
    girar_esquerda(90);
    mover_tempo(-300, 511);
    preparar_atuador();
    timeout = millis() + 3000;
    while (!tem_vitima() && millis() < timeout)
    {
        mover(300, 300);
    }
    fechar_atuador();
    levantar_atuador();
    parar();
    if (direcao_triangulo == 2)
    {
        objetivo_esquerda(converter_graus(direcao_inicial + 45));
        mover(300, 300);
    }
}

void mover_travar_tempo(short velocidade = 300, short _timeout = 3000)
{
    /*
    Responsável por mover o robô até ser interrompido por algo externo
        - Move na velocidade indicada
        - Evita problemas com inclinações indesejadas causadas por vítimas
        - Para o robô caso os motores sejam travados ou haja uma alteração grande no ângulo
    */
    // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
    short angulo_inicial = eixo_x();
    // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
    int tempo_check = millis() + 200;
    // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
    bool flag_check = false;
    levantar_atuador();
    timeout = millis() + _timeout;
    while (millis() < timeout)
    {
        // Move o robô
        mover(velocidade, velocidade);
        // Verifica e evita inclinações indesejadas
        check_subida_frente();
        if (!flag_check && millis() > tempo_check)
        {
            // Se a flag era falsa e já passou o tempo inicial
            // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
            flag_check = true;
        }
        if (flag_check && (forca_motor() < 0.3 || !proximo(eixo_x(), angulo_inicial, 3)))
        {
            // Se a flag ja for verdadeira e a força atual for menor que 0.3 ou o angulo atual for muito diferente do angulo inicial
            // Para o loop
            break;
        }
    }
    // Para o robô e alinha o robô no ângulo reto mais próximo
    alinhar_angulo();
}

void mover_travar_ultra(short velocidade = 300, short alvo = 25)
{
    /*
    Responsável por mover o robô até chegar no alvo desejado do ultrassônico
        - Move na velocidade indicada
        - Evita problemas com inclinações indesejadas causadas por vítimas
        - Para o robô caso os motores sejam travados ou haja uma alteração grande no ângulo
    */
    // Define o ângulo inicial do robô para fazer a comparação com o ângulo durante o movimento
    short angulo_inicial = eixo_x();
    // Seta o tempo inicial como 200ms, esse é o tempo destinado para o robô sair da inércia
    int tempo_check = millis() + 200;
    // Flag de verificações configurada como falso, quando ele passar do tempo_check será verdadeiro
    bool flag_check = false;
    levantar_atuador();
    while (ultra(1) > alvo)
    {
        // Move o robô
        mover(velocidade, velocidade);
        // Verifica e evita inclinações indesejadas
        check_subida_frente();
        if (!flag_check && millis() > tempo_check)
        {
            // Se a flag era falsa e já passou o tempo inicial
            // Seta a flag como verdadeiro e ele ja começa a verificar o travamento
            flag_check = true;
        }
        if (flag_check && (forca_motor() < 0.3 || !proximo(eixo_x(), angulo_inicial, 3)))
        {
            // Se a flag ja for verdadeira e a força atual for menor que 0.3 ou o angulo atual for muito diferente do angulo inicial
            // Para o loop
            break;
        }
    }
    // Para o robô e alinha o robô no ângulo reto mais próximo
    alinhar_angulo();
}

void meio_triangulo()
{
    print(2, "Alinhando para o triângulo");
    girar_direita(90);
    alinhar_ultra(124);
    print(2, "Girando para o triângulo");
    girar_direita(45);
    print(2, "Indo para o triângulo");
    while (ultra(0) > 80)
    {
        mover(300, 300);
    }
    mover_tempo(250, 500);
    print(2, "Entregando vítima");
    entregar_vitima();
    print(1, "Voltando à busca");
    print(2, "Indo ao meio");
    while (ultra(0) < 175)
    {
        mover(-300, -300);
    }
    print(2, "Alinhando...");
    girar_esquerda(45);
    alinhar_angulo();
    alinhar_ultra(124);
    girar_esquerda(90);
}

void alcancar_saida()
{
    mover_tempo(300, 500);
    mover_tempo(-300, 500);
    parar();
    abaixar_atuador();
    delay(300);
    while (!verde(0) && !verde(1) && !verde(2) && !verde(3))
    {
        mover(300, 300);
    }
    limpar_console();
    print(2, "Saindo!");
    som("C2", 100);

    while (verde(0) || verde(1) || verde(2) || verde(3))
        mover(200, 200);
    delay(150);
    parar();
    mover(200, 200);
    delay(16);
    parar();
    delay(300);
    lugar = 3;
}
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
void varredura()
{
    direcao_inicial = eixo_x();
    mover_para(1, 5);
}


// Variáveis de controle para ligar/desligar o debug e console
bool debug = false;
bool console = true;

// Método principal
void Main()
{
    if (debug)
    {
        print(2, "debug");
        travar();
    }
    else
    {
        calibrar();
        ultima_correcao = millis();
        fechar_atuador();
        abaixar_atuador();
        console_led(3, "<:Local atual: PISO:>", "cinza claro", false);
        while (lugar == 0)
        {
            print_luz_marker();
            verifica_obstaculo();
            verifica_saida();
            seguir_linha();
            verifica_calibrar();
            verifica_rampa();
            verifica_rampa_resgate();
        }
        travar();
    }
}
