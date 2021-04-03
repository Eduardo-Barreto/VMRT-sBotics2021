bool debug = false;

bc.actuatorSpeed(150);
bc.actuatorUp(100);

calibrar();

if(bc.angleActuator() >= 0 && bc.angleActuator() < 88){
    bc.actuatorSpeed(150);
    bc.actuatorUp(600);
}

while(!debug){
    verifica_calibrar();
    seguir_linha();
    verifica_curva();
}


while(debug){
    print(1, "DEBUG");
    print(2, bc.angleActuator());
}
