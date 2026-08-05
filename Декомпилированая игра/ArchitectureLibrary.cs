public class ArchitectureLibrary : AssetLibrary<ArchitectureAsset>
{
	private const string TEMPLATE_WITH_GENERATED_BUILDINGS = "$template_with_generated_buildings$";

	public override void init()
	{
		base.init();
		addTemplates();
		addClassic();
		addUnique();
		addAnimal();
	}

	private void addTemplates()
	{
		add(new ArchitectureAsset
		{
			id = "$template_with_generated_buildings$",
			generation_target = "human",
			generate_buildings = true
		});
		t.styled_building_orders = new string[9] { "order_docks_0", "order_docks_1", "order_house_0", "order_hall_0", "order_windmill_0", "order_watch_tower", "order_temple", "order_library", "order_barracks" };
		t.shared_building_orders = new(string, string)[6]
		{
			("order_bonfire", "bonfire"),
			("order_statue", "statue"),
			("order_well", "well"),
			("order_stockpile", "stockpile"),
			("order_mine", "mine"),
			("order_training_dummy", "training_dummy")
		};
		t.actor_asset_id_trading = "boat_trading_human";
		t.actor_asset_id_transport = "boat_transport_human";
	}

	private void addClassic()
	{
		add(new ArchitectureAsset
		{
			id = "human"
		});
		t.actor_asset_id_trading = "boat_trading_human";
		t.actor_asset_id_transport = "boat_transport_human";
		t.styled_building_orders = new string[19]
		{
			"order_docks_0", "order_tent", "order_house_0", "order_house_1", "order_house_2", "order_house_3", "order_house_4", "order_house_5", "order_hall_0", "order_hall_1",
			"order_hall_2", "order_windmill_0", "order_windmill_1", "order_docks_1", "order_watch_tower", "order_temple", "order_library", "order_market", "order_barracks"
		};
		t.shared_building_orders = new(string, string)[6]
		{
			("order_bonfire", "bonfire"),
			("order_statue", "statue"),
			("order_well", "well"),
			("order_stockpile", "stockpile"),
			("order_mine", "mine"),
			("order_training_dummy", "training_dummy")
		};
		clone("orc", "human");
		t.actor_asset_id_trading = "boat_trading_orc";
		t.actor_asset_id_transport = "boat_transport_orc";
		clone("elf", "human");
		t.actor_asset_id_trading = "boat_trading_elf";
		t.actor_asset_id_transport = "boat_transport_elf";
		clone("dwarf", "human");
		t.actor_asset_id_trading = "boat_trading_dwarf";
		t.actor_asset_id_transport = "boat_transport_dwarf";
	}

	private void addUnique()
	{
		clone("civ_necromancer", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_necromancer";
		t.actor_asset_id_transport = "boat_transport_necromancer";
		t.spread_biome_id = "biome_corrupted";
		clone("civ_alien", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_alien";
		t.actor_asset_id_transport = "boat_transport_alien";
		t.projectile_id = "plasma_ball";
		clone("civ_druid", "$template_with_generated_buildings$");
		t.spread_biome_id = "biome_jungle";
		t.actor_asset_id_trading = "boat_trading_druid";
		t.actor_asset_id_transport = "boat_transport_druid";
		clone("civ_bee", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_bee";
		t.actor_asset_id_transport = "boat_transport_bee";
		clone("civ_beetle", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_beetle";
		t.actor_asset_id_transport = "boat_transport_beetle";
		clone("civ_seal", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_seal";
		t.actor_asset_id_transport = "boat_transport_seal";
		clone("civ_unicorn", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_unicorn";
		t.actor_asset_id_transport = "boat_transport_unicorn";
		clone("civ_ghost", "$template_with_generated_buildings$");
		t.has_shadows = false;
		t.material = "jelly";
		t.actor_asset_id_trading = "boat_trading_ghost";
		t.actor_asset_id_transport = "boat_transport_ghost";
		clone("civ_fairy", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_fairy";
		t.actor_asset_id_transport = "boat_transport_fairy";
		clone("civ_evil_mage", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_evil_mage";
		t.actor_asset_id_transport = "boat_transport_evil_mage";
		t.replaceSharedID("order_stockpile", "stockpile_fireproof");
		t.burnable_buildings = false;
		clone("civ_white_mage", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_white_mage";
		t.actor_asset_id_transport = "boat_transport_white_mage";
		clone("civ_bandit", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_bandit";
		t.actor_asset_id_transport = "boat_transport_bandit";
		clone("civ_demon", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_demon";
		t.actor_asset_id_transport = "boat_transport_demon";
		t.burnable_buildings = false;
		t.spread_biome_id = "biome_infernal";
		t.replaceSharedID("order_stockpile", "stockpile_fireproof");
		clone("civ_cold_one", "$template_with_generated_buildings$");
		t.spread_biome_id = "biome_permafrost";
		t.actor_asset_id_trading = "boat_trading_cold_one";
		t.actor_asset_id_transport = "boat_transport_cold_one";
		clone("civ_angle", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_angle";
		t.actor_asset_id_transport = "boat_transport_angle";
		clone("civ_snowman", "$template_with_generated_buildings$");
		t.spread_biome_id = "biome_permafrost";
		t.actor_asset_id_trading = "boat_trading_snowman";
		t.actor_asset_id_transport = "boat_transport_snowman";
		clone("civ_garlic_man", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_garlic_man";
		t.actor_asset_id_transport = "boat_transport_garlic_man";
		t.spread_biome_id = "biome_garlic";
		clone("civ_lemon_man", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_lemon_man";
		t.actor_asset_id_transport = "boat_transport_lemon_man";
		t.spread_biome_id = "biome_lemon";
		clone("civ_acid_gentleman", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_acid_gentleman";
		t.actor_asset_id_transport = "boat_transport_acid_gentleman";
		t.spread_biome_id = "biome_wasteland";
		t.acid_affected_buildings = false;
		t.replaceSharedID("order_stockpile", "stockpile_acidproof");
		clone("civ_crystal_golem", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_crystal_golem";
		t.actor_asset_id_transport = "boat_transport_crystal_golem";
		t.spread_biome_id = "biome_crystal";
		t.burnable_buildings = false;
		clone("civ_candy_man", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_candy_man";
		t.actor_asset_id_transport = "boat_transport_candy_man";
		t.spread_biome_id = "biome_candy";
		clone("civ_liliar", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_liliar";
		t.actor_asset_id_transport = "boat_transport_liliar";
		t.spread_biome_id = "biome_flower";
		clone("civ_greg", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_greg";
		t.actor_asset_id_transport = "boat_transport_greg";
	}

	private void addAnimal()
	{
		clone("civ_cat", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_cat";
		t.actor_asset_id_transport = "boat_transport_cat";
		clone("civ_dog", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_dog";
		t.actor_asset_id_transport = "boat_transport_dog";
		clone("civ_chicken", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_chicken";
		t.actor_asset_id_transport = "boat_transport_chicken";
		clone("civ_rabbit", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_rabbit";
		t.actor_asset_id_transport = "boat_transport_rabbit";
		clone("civ_monkey", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_monkey";
		t.actor_asset_id_transport = "boat_transport_monkey";
		clone("civ_fox", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_fox";
		t.actor_asset_id_transport = "boat_transport_fox";
		clone("civ_sheep", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_sheep";
		t.actor_asset_id_transport = "boat_transport_sheep";
		clone("civ_cow", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_cow";
		t.actor_asset_id_transport = "boat_transport_cow";
		clone("civ_armadillo", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_armadillo";
		t.actor_asset_id_transport = "boat_transport_armadillo";
		clone("civ_wolf", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_wolf";
		t.actor_asset_id_transport = "boat_transport_wolf";
		clone("civ_bear", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_bear";
		t.actor_asset_id_transport = "boat_transport_bear";
		clone("civ_rhino", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_rhino";
		t.actor_asset_id_transport = "boat_transport_rhino";
		clone("civ_buffalo", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_buffalo";
		t.actor_asset_id_transport = "boat_transport_buffalo";
		clone("civ_hyena", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_hyena";
		t.actor_asset_id_transport = "boat_transport_hyena";
		clone("civ_rat", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_rat";
		t.actor_asset_id_transport = "boat_transport_rat";
		clone("civ_alpaca", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_alpaca";
		t.actor_asset_id_transport = "boat_transport_alpaca";
		clone("civ_capybara", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_capybara";
		t.actor_asset_id_transport = "boat_transport_capybara";
		clone("civ_goat", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_goat";
		t.actor_asset_id_transport = "boat_transport_goat";
		clone("civ_scorpion", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_scorpion";
		t.actor_asset_id_transport = "boat_transport_scorpion";
		clone("civ_crab", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_crab";
		t.actor_asset_id_transport = "boat_transport_crab";
		clone("civ_penguin", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_penguin";
		t.actor_asset_id_transport = "boat_transport_penguin";
		t.spread_biome_id = "biome_permafrost";
		clone("civ_turtle", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_turtle";
		t.actor_asset_id_transport = "boat_transport_turtle";
		clone("civ_crocodile", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_crocodile";
		t.actor_asset_id_transport = "boat_transport_crocodile";
		clone("civ_snake", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_snake";
		t.actor_asset_id_transport = "boat_transport_snake";
		clone("civ_frog", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_frog";
		t.actor_asset_id_transport = "boat_transport_frog";
		clone("civ_piranha", "$template_with_generated_buildings$");
		t.actor_asset_id_trading = "boat_trading_piranha";
		t.actor_asset_id_transport = "boat_transport_piranha";
	}

	public override void post_init()
	{
		base.post_init();
		foreach (ArchitectureAsset tAsset in list)
		{
			if (!string.IsNullOrEmpty(tAsset.spread_biome_id))
			{
				tAsset.spread_biome = true;
			}
		}
		initBuildingKeys();
	}

	private void initBuildingKeys()
	{
		foreach (ArchitectureAsset tAsset in list)
		{
			if (!tAsset.isTemplateAsset())
			{
				loadAutoBuildingsForAsset(tAsset);
				(string, string)[] shared_building_orders = tAsset.shared_building_orders;
				for (int i = 0; i < shared_building_orders.Length; i++)
				{
					(string, string) tSharedBuilding = shared_building_orders[i];
					tAsset.addBuildingOrderKey(tSharedBuilding.Item1, tSharedBuilding.Item2);
				}
			}
		}
	}

	private void loadAutoBuildingsForAsset(ArchitectureAsset pAsset)
	{
		string tArchitectureID = pAsset.id;
		string[] styled_building_orders = pAsset.styled_building_orders;
		foreach (string tOrderID in styled_building_orders)
		{
			string tBuildCivID = null;
			switch (tOrderID)
			{
			case "order_tent":
				tBuildCivID = "tent_" + tArchitectureID;
				break;
			case "order_house_0":
			case "order_house_1":
			case "order_house_2":
			case "order_house_3":
			case "order_house_4":
			case "order_house_5":
				tBuildCivID = "house_" + tArchitectureID + "_" + tOrderID.Substring(tOrderID.Length - 1);
				break;
			case "order_hall_0":
			case "order_hall_1":
			case "order_hall_2":
				tBuildCivID = "hall_" + tArchitectureID + "_" + tOrderID.Substring(tOrderID.Length - 1);
				break;
			case "order_windmill_0":
			case "order_windmill_1":
				tBuildCivID = "windmill_" + tArchitectureID + "_" + tOrderID.Substring(tOrderID.Length - 1);
				break;
			case "order_docks_0":
				tBuildCivID = "fishing_docks_" + tArchitectureID;
				break;
			case "order_docks_1":
				tBuildCivID = "docks_" + tArchitectureID;
				break;
			case "order_watch_tower":
				tBuildCivID = "watch_tower_" + tArchitectureID;
				break;
			case "order_temple":
				tBuildCivID = "temple_" + tArchitectureID;
				break;
			case "order_library":
				tBuildCivID = "library_" + tArchitectureID;
				break;
			case "order_market":
				tBuildCivID = "market_" + tArchitectureID;
				break;
			case "order_barracks":
				tBuildCivID = "barracks_" + tArchitectureID;
				break;
			case "order_bonfire":
				tBuildCivID = "bonfire";
				break;
			case "order_statue":
				tBuildCivID = "statue";
				break;
			case "order_well":
				tBuildCivID = "well";
				break;
			case "order_mine":
				tBuildCivID = "mine";
				break;
			case "order_stockpile":
				tBuildCivID = "stockpile";
				break;
			case "order_training_dummy":
				tBuildCivID = "training_dummy";
				break;
			}
			if (tBuildCivID != null)
			{
				pAsset.addBuildingOrderKey(tOrderID, tBuildCivID);
			}
		}
	}
}
