# AnimationModelingDelivery3
--------------------------------------------------------------------------

Descripción del grupo: Estudiantes de CDI en 3º de ENTI-UB.

Pablo Perpiñan Cutillas (pablo.perpinan@enti.cat)

Josep Romera Andreu (josep.romera@enti.cat)

Ivan Sales Méndez (ivan.sales@enti.cat)

--------------------------------------------------------------------------

Controles:
    R: Reinicia la escena
    I: Activar/Desactivar guiar
    
--------------------------------------------------------------------------

--------------------------------------------------------------------------

Scripts:
    Ejercicio 1:
        1.1.- IK_tentacles y RestartController
        1.2.- MovingBall
        1.3.- IK_Scorpion, MovingBall (En el objeto slider_force estan los valores que puede tomar)
        1.4.- ResetController (en el objeto Restart de la escena)

    Ejercicio 2:
        2.1.- MagnusSliderController (En el objeto slider_force estan los valores que puede tomar), IK_Scorpion
        2.2.- MovingBall (Funcion EulerStep)
        2.3.- MovingBall (Se han agregado las flechas y particulas a la escena, la previsualizacion del magnus effect no la acaba de hacer del todo bien en algunos casos)
        2.4.- MovingBall (Se han agregado las flechas y particulas a la escena, la previsualizacion del magnus effect no la acaba de hacer del todo bien en algunos casos)

    Ejercicio 4:
        4.1.- IK_Scorpion

--------------------------------------------------------------------------

Explicaciones:
    1.5.- Se ha utilizado Euler Steps para calcular la trayectoria de la pelota hacia el target. En este, se suman la fuerza que recibe del golpe multiplicada por la dirección normalizada hacia el target, esto sumado a la aceleración (la cual es de -9.81 de gravedad (para que esta no afecte demasiado y acabe llegando más o menos al target indicado, y la masa de la pelota que es de 1) multiplicada por el tiempo.

--------------------------------------------------------------------------