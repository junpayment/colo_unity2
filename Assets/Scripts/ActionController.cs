using UnityEngine;
using System.Collections;

public class ActionController : MonoBehaviour {

	// public
	public GameObject rest;
	public GameObject player;
	public GameObject enemy;
	public GameObject damage;
	public GameObject player_hp;
	public GameObject enemy_hp;
	public int hp_max = 1000;
	public float refresh = 0.5f;
	public GameObject dp;
	public GameObject wp;
	public GameObject waza;

	// enum
	public enum command{
		idle,
		hadoken,
		shoryuken,
		tasumaki,
	}

	// private
	private int player_hp_num;
	private int enemy_hp_num;
	private Queue cmd_que;
	private command status;
	private float time;

	// Use this for initialization
	void Start () {
		this.cmd_que = new Queue();
		this.status = command.idle;
		this.time = 0f;
		this.enemy_hp_num = this.hp_max;
	}
	
	// Update is called once per frame
	void Update () {
		time += Time.deltaTime;

		if(time >= refresh)
		{
			if( this.enemy_hp_num <= 0){
				Application.LoadLevel("start");
			}

			time = 0f;

			// exec if cmds exist in que
			if(cmd_que.Count > 0)
			{
				command cmd = (command)cmd_que.Dequeue();
				this._setStatus(cmd);
			}
			else
			{
				// set idle when no cmd in que
				this._setStatus(command.idle);
			}

			// change texture
			UISprite us_player = player.GetComponent<UISprite>();
			UISprite us_enemy = enemy.GetComponent<UISprite>();

			switch(this.status){
			case command.idle:
				us_player.spriteName = "idle";
				us_enemy.spriteName = "idle";
				break;

			case command.hadoken:
				us_player.spriteName = "hado";
				us_enemy.spriteName = "damage";
				damage_enemy("はどーけん");
				break;

			case command.shoryuken:
				us_player.spriteName = "shoryu";
				us_enemy.spriteName = "damage";
				damage_enemy("しょーりゅーけん");

				break;

			case command.tasumaki:
				us_player.spriteName = "tatsumaki";
				us_enemy.spriteName = "damage";
				damage_enemy("まきせんぷーきゃ");

				break;

			default:
				break;
			}

			// change hp
			UILabel label = enemy_hp.GetComponent<UILabel>();
			label.text = this.enemy_hp_num.ToString();

		}

	}

	// function
	/// <summary>
	/// input comman
	/// </summary>
	/// <param name="cmd">Cmd.</param>
	public void input(command cmd)
	{
		cmd_que.Enqueue(cmd);
	}

	// exec cmd
	private void _setStatus(command cmd)
	{
		this.status = cmd;
	}

	// damage
	private void damage_enemy(string str)
	{
		int dm = 100;
		if(this.enemy_hp_num>0){
			this.enemy_hp_num -= dm;
		}else{
			this.enemy_hp_num = 0;
		}

		GameObject dd = (GameObject)Instantiate(this.damage);
		GameObject.Destroy(dd,1.0f);
		UILabel label = dd.GetComponent<UILabel>();
		label.text = dm.ToString();
		dd.transform.parent = this.dp.transform;
		dd.transform.localPosition = Vector3.zero;
		dd.transform.localScale = new Vector3(80,80,1);


		GameObject cm = (GameObject)Instantiate(this.waza);
		GameObject.Destroy(cm,1.0f);
		cm.transform.parent = this.wp.transform;
		cm.transform.localPosition = Vector3.zero;
		cm.transform.localScale = new Vector3(1,1,1);



	}

}
