using Godot;
using System;

public class Player : KinematicBody
{
    float camera_angle = 0F;
    float mouse_sensitivity = 0.3F;
    Vector2 camera_change = new Vector2();
    
    Vector3 velocity = new Vector3();
    Vector3 direction = new Vector3();
    
    // walk variables
    float gravity = -9.8F * 3;
    int max_speed = 30;
    int acceleration = 5;

    // jumping
    int jump_height = 10;

    // c# specific
    Spatial feet; 
    Spatial head;
    Camera camera;

    public override void _Ready()
    {
        // Called every time the node is added to the scene.
        // Initialization here
        feet = (Spatial)GetNode("Tail");
        head = (Spatial)GetNode("Head");
        camera = (Camera)head.GetNode("Camera");
    }

    public override void _PhysicsProcess(float delta)
    {
        Aim();
        Move(delta);
    }

    public override void _Input(InputEvent e)
    {
        if (e is InputEventMouseMotion em)
        {
            camera_change = em.Relative;
        }
    }
    
    public void Move(float delta)
    {
        // reset the direction of the player
        direction = new Vector3();

        // get the rotation of the camera
        Basis aim = camera.GetGlobalTransform().basis;
                
        // check input and change direction
        if (Input.IsActionPressed("move_forward"))
        {
            direction -= aim.z;
        }
        if (Input.IsActionPressed("move_backward"))
        {
            direction += aim.z;
        }
        if (Input.IsActionPressed("move_left"))
        {
            direction -= aim.x;
        }
        if (Input.IsActionPressed("move_right"))
        {
            direction += aim.x;
        }
        direction.y = 0;
        direction = direction.Normalized();
        velocity.y += gravity * delta;

        Vector3 temp_velocity = velocity;
        temp_velocity.y = 0;

        // where would the player go at max speed
        Vector3 target = direction * max_speed;

        // calculate a portion of the distance to go
        temp_velocity = temp_velocity.LinearInterpolate(target, acceleration * delta);
        velocity.x = temp_velocity.x;
        velocity.z = temp_velocity.z;

        // jump
        if (Input.IsActionJustPressed("jump"))
        {
            velocity.y = jump_height;
        }

        // move
        velocity = MoveAndSlide(velocity, new Vector3(0,1,0));
    }
    public void Aim()
    {
        if (camera_change.Length() > 0)
        {          
            head.RotateY(Mathf.Deg2Rad(-camera_change.x * mouse_sensitivity));

            // limit how far up/down we look
            float change = -camera_change.y * mouse_sensitivity;
            if (camera_angle + change < 90F && camera_angle + change > -90F)
            {
                // invert mouse (-change)
                camera.RotateX(Mathf.Deg2Rad(-change));
                camera_angle += change;
            }
            camera_change = new Vector2();
        }
    }
}