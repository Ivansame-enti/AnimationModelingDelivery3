# AnimationModelingDelivery3
--------------------------------------------------------------------------

Descripción del grupo: Estudiantes de CDI en 3º de ENTI-UB.

Pablo Perpiñan Cutillas (pablo.perpinan@enti.cat)

Josep Romera Andreu (josep.romera@enti.cat)

Ivan Sales Méndez (ivan.sales@enti.cat)

--------------------------------------------------------------------------

Controles:
    R: Reinicia la escena
    
--------------------------------------------------------------------------

--------------------------------------------------------------------------

Scripts:
    Ejercicio 1:
        1.1.- IK_tentacles y RestartController
        1.2.- MovingBall
        1.3.- IK_Scorpion, MovingBall (En el objeto slider_force estan los valores que puede tomar)
        1.4.- ResetController (en el objeto Restart de la escena)

    Ejercicio 2:
        2.1.- MagnusSliderController, IK_Scorpion

    Ejercicio 4:
        4.1.- IK_Scorpion

--------------------------------------------------------------------------

Explicaciones:
    1.5.- Se ha utilizado Euler Steps para calcular la trayectoria de la pelota hacia el target. En este, se suman la fuerza que recibe del golpe multiplicada por la dirección normalizada, más la aceleración (la cual es de -1 de gravedad (para que esta no afecte demasiado y acabe llegando más o menos al target indicado, y la masa de la pelota que es de 1) multiplicada por el tiempo.

--------------------------------------------------------------------------