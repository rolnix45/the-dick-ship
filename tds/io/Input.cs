using Microsoft.Xna.Framework.Input;

namespace ahn.io;

internal static class Input
{
    private static KeyboardState key_state, prev_key_state = Keyboard.GetState();
    private static MouseState m_state, prev_m_state = Mouse.GetState();

    public static void Update()
    {
        prev_key_state = key_state;
        prev_m_state = m_state;
        key_state = Keyboard.GetState();
        m_state = Mouse.GetState();
    }

    public static bool KeyPressed(Keys k) =>
        key_state.IsKeyDown(k) && !prev_key_state.IsKeyDown(k);

    public static bool KeyHeld(Keys k) =>
        key_state.IsKeyDown(k);

    public static bool LMBPressed() =>
        m_state.LeftButton == ButtonState.Pressed && prev_m_state.LeftButton != ButtonState.Pressed;
    
    public static bool RMBPressed() =>
        m_state.RightButton == ButtonState.Pressed && prev_m_state.RightButton != ButtonState.Pressed;

    public static bool LMBHeld() =>
        m_state.LeftButton == ButtonState.Pressed;

    public static bool RMBHeld() =>
        m_state.RightButton == ButtonState.Pressed;
}