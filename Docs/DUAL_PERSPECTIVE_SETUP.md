# Arquitectura de doble perspectiva (2D / 3D)

Esta primera integración añade un conmutador de perspectiva sin reemplazar los sistemas 2D existentes.

## Archivos añadidos

- `Assets/Scripts/Core/GamePerspective.cs`: enum compartido `TwoD` / `ThreeD`.
- `Assets/Scripts/Core/PerspectiveSwitcher.cs`: activa una perspectiva, desactiva la otra y mueve al jugador a su punto de entrada equivalente.

## Estructura recomendada de escena

Crea un objeto vacío `PerspectiveSystem` y añade `PerspectiveSwitcher`.

Organiza la escena con raíces separadas:

```text
Scene
├── PerspectiveSystem
├── Player
├── World2D
│   ├── Camera2D
│   ├── Rooms2D
│   └── Enemies2D
├── World3D
│   ├── Camera3D
│   ├── Geometry3D
│   └── Enemies3D
└── EntryPoints
    ├── Entry2D
    └── Entry3D
```

No es obligatorio usar exactamente estos nombres. Lo importante es que las referencias del Inspector apunten a los objetos correctos.

## Configuración del Inspector

En `PerspectiveSwitcher`:

1. **Player**: arrastra el Transform del jugador que se reposicionará al cambiar.
2. **Character Controller**: arrastra el CharacterController si el jugador 3D lo utiliza. Se desactiva brevemente durante el teletransporte para evitar conflictos.
3. **Two D**:
   - World Root: `World2D`.
   - Perspective Camera: `Camera2D`.
   - Player Entry Point: `Entry2D`.
   - Enabled Behaviours: controlador de movimiento 2D, disparo 2D y seguimiento de cámara si esos componentes deben habilitarse/deshabilitarse con el modo.
4. **Three D**:
   - World Root: `World3D`.
   - Perspective Camera: `Camera3D`.
   - Player Entry Point: `Entry3D`.
   - Enabled Behaviours: `FirstPersonController` y el controlador de disparo 3D cuando exista.
5. **Starting Perspective**: elige el modo inicial.
6. **Switch Key**: por defecto `Tab`.

Los dos puntos de entrada deben representar el mismo punto lógico de la partida en cada mundo. No tienen por qué compartir coordenadas.

## Reglas importantes

- Mantén los mundos 2D y 3D como geometrías separadas. Este sistema no convierte sprites, colliders 2D ni enemigos 2D en objetos 3D.
- Los componentes que aparecen en `Enabled Behaviours` deben ser controladores exclusivos de ese modo. No incluyas aquí el propio `PerspectiveSwitcher` ni sistemas compartidos.
- Si el jugador 2D usa Rigidbody2D y el jugador 3D usa CharacterController, no asumas que ambos componentes pueden controlar simultáneamente al mismo objeto. Para una implementación robusta, usa representaciones físicas separadas o un controlador compartido diseñado para ambos modos.
- Configura el proyecto en **Active Input Handling = Both** si los controladores usan `UnityEngine.Input` clásico y el proyecto también tiene Input System habilitado.
- Prueba el cambio en Play Mode y comprueba cámara, posición, colisiones, enemigos y entradas antes de integrar progresión o combate compartido.

## Alcance de esta entrega

Esta es la base de arquitectura y el cambio de modo. No modifica todavía las escenas, prefabs ni scripts 2D existentes, y no crea conversión automática de enemigos, proyectiles, habitaciones o progreso entre dimensiones.
