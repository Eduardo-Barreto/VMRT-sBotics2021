bool debug = false;

calibrar();

if(bc.angleActuator() >= 0 && bc.angleActuator() < 89){
    bc.actuatorSpeed(150);
    bc.actuatorUp(600);
}

while(!debug){
    verifica_calibrar();
    seguir_linha();
}


while(debug){
    print(1, luz(0));
}
