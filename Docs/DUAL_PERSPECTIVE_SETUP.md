# Arquitectura de doble perspectiva 2D / 3D para Android

La partida se desarrolla principalmente en 2D. Al entrar en una sala especial, cambia automáticamente a 3D en primera persona. Al salir de esa sala, vuelve a 2D. No se necesita un botón para cambiar de perspectiva.

## Archivos de esta rama

- `Assets/Scripts/Core/GamePerspective.cs`: enum compartido `TwoD` / `ThreeD`.
- `Assets/Scripts/Core/PerspectiveSwitcher.cs`: activa una perspectiva, su cámara y sus controladores; reposiciona al jugador en el punto equivalente.
- `Assets/Scripts/Core/PerspectiveRoomZone.cs`: detecta la entrada 2D a la sala especial y la salida 3D de esa sala.
- Este documento: configuración de escena.

## Estructura recomendada

```text
Scene
├── PerspectiveSystem
│   └── PerspectiveSwitcher
├── Player                         (no debe estar dentro de World2D ni World3D)
├── World2D                        (se activa fuera de la sala especial)
│   ├── Camera2D
│   └── Rooms2D
├── World3D                        (se activa dentro de la sala especial)
│   ├── Camera3D
│   └── SpecialRoomGeometry
├── PersistentRoomTriggers         (siempre activo; fuera de ambas raíces)
│   └── SpecialRoomZone
└── EntryPoints
    ├── Entry2D                     (en el pasillo, fuera del trigger 2D)
    └── Entry3D                     (dentro del volumen de la sala 3D)
```

Los nombres son orientativos. Lo esencial es mantener el jugador, PerspectiveSystem y los detectores de sala fuera de las raíces que se apagan y encienden.

## Configurar PerspectiveSwitcher

Añade el componente a un GameObject persistente llamado, por ejemplo, `PerspectiveSystem`.

- **Player**: Transform raíz del jugador.
- **Character Controller**: CharacterController del jugador 3D, si se usa.
- **Starting Perspective**: `TwoD`.
- **Allow Keyboard Switch**: desactivado. Es solo una opción temporal de depuración en PC; el cambio normal lo realizan las zonas de sala.

Configuración **Two D**:
- World Root: raíz `World2D`.
- Perspective Camera: cámara 2D.
- Player Entry Point: `Entry2D`.
- Enabled Behaviours: componentes exclusivos del control 2D, por ejemplo movimiento, disparo y seguimiento de cámara.

Configuración **Three D**:
- World Root: raíz `World3D`.
- Perspective Camera: cámara FPS.
- Player Entry Point: `Entry3D`.
- Enabled Behaviours: controlador FPS y disparo 3D cuando estén implementados.

No incluyas componentes compartidos ni el propio PerspectiveSwitcher en las listas de controladores exclusivos.

## Configurar la sala especial

1. Crea un GameObject `SpecialRoomZone` fuera de `World2D` y `World3D`. Debe permanecer activo en ambas perspectivas.
2. Añade `PerspectiveRoomZone`.
3. El componente requiere un `BoxCollider2D` y un `BoxCollider`; ambos se configuran como Trigger automáticamente.
4. Ajusta el **BoxCollider2D** para cubrir la entrada de la sala en el plano 2D. Al entrar el jugador, se solicita la perspectiva 3D.
5. Ajusta el **BoxCollider 3D** para cubrir el espacio interior de la sala 3D. Al salir el jugador de ese volumen, se solicita la perspectiva 2D.
6. Asigna **Perspective Switcher** y **Player Root** en el Inspector.
7. Deja **Perspective Inside Room = ThreeD** y **Perspective Outside Room = TwoD**.
8. Coloca `Entry3D` dentro del volumen 3D y `Entry2D` fuera del trigger de entrada 2D, en el pasillo al que debe regresar el jugador.

Importante: el trigger 2D y el trigger 3D pueden necesitar posiciones, tamaños y orientaciones diferentes. Ambos pertenecen al mismo objeto persistente, pero cada collider se ajusta a la geometría de su dimensión.

El detector procesa la entrada 2D y la salida 3D deliberadamente. No procesa `OnTriggerExit2D`: al cambiar de modo se apaga el mundo 2D, y ese apagado no debe provocar una vuelta inmediata a 2D.

## Requisitos de colisiones

- En 2D, el jugador debe tener un `Collider2D` y normalmente un `Rigidbody2D` para generar eventos de trigger 2D.
- En 3D, el jugador debe tener un `CharacterController` o un `Rigidbody` con collider para generar eventos de trigger 3D.
- Comprueba la matriz de colisiones de Physics 2D y Physics para que el jugador pueda detectar los triggers.
- El objeto `SpecialRoomZone` no debe ser hijo de una raíz que se desactive al cambiar de perspectiva.
- Si el jugador tiene colliders en objetos hijos, asigna como Player Root el Transform común que los contiene.

## Consideraciones Android

Esta arquitectura no usa un botón de teclado para cambiar de perspectiva. El cambio ocurre por proximidad a la sala, así que también funciona con pantalla táctil.

Los controles de movimiento y apuntado sí deben ser táctiles en Android. El controlador FPS que actualmente usa ejes de teclado/mouse necesita conectarse a controles virtuales (por ejemplo, joystick izquierdo, zona táctil derecha para mirar y botón de disparo). No se debe considerar terminado el soporte Android hasta probar esos controles en un dispositivo real.

Para rendimiento móvil:
- Mantén solo una perspectiva y su cámara activas a la vez.
- Evita activar/desactivar objetos compartidos con cada transición.
- Usa geometría, materiales, luces y efectos moderados en la sala 3D.
- Prueba tiempos de transición, consumo y temperatura en un teléfono Android real.

## Alcance actual y pruebas pendientes

Esta rama añade la base de cambio por zona, pero no modifica automáticamente las escenas ni los prefabs existentes. Tampoco convierte enemigos, proyectiles, inventario o progreso entre 2D y 3D.

Antes de integrar a `main`, configurar en Unity 6.6 y comprobar:
- [ ] El juego inicia en 2D.
- [ ] Entrar en el trigger 2D activa la sala, cámara y controles 3D.
- [ ] El jugador aparece en `Entry3D`.
- [ ] Salir del volumen 3D restaura la perspectiva y controles 2D.
- [ ] No hay doble movimiento, cámara duplicada ni transición repetida.
- [ ] Las transiciones funcionan en Android y los controles táctiles responden.
