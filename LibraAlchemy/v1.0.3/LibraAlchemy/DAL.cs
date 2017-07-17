using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.SqlServerCe;
using System.IO;
using System.Data;


namespace LibraAlchemy
{
    class DAL
    {
        //Need to do something about this address...
        private static string DB_ADDRESS = "datasource="+"C:\\Users\\Aviad\\documents\\visual studio 2012\\Projects\\LibraAlchemy\\LibraAlchemy\\Database1.sdf";

        //private static string DB_ADDRESS = "Data Source=" + System.IO.Path.Combine(Path.GetDirectoryName(System.Reflection.Assembly.GetEntryAssembly().Location), "Database1.sdf");

        public static string IDListToString(List<int> l)
        {
            string id_list = "";
            for (int i = 0; i < l.Count - 1; i++)
            {
                id_list += l[i].ToString() + ",";
            }
            id_list += l.Last().ToString();
            return id_list;
        }

        //
        //Ingredient Functions
        //
        public static Ingredient GetIngredient(int ID)
        {
            Ingredient ing = new Ingredient();
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Ingredient WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", ID);
            con.Open();
            dr = cm.ExecuteReader();
            if (dr.Read())
            {
                ing.ID = int.Parse(dr["ID"].ToString());
                ing.Name = dr["Name"].ToString();
                ing.Comment = dr["Comment"].ToString();
            }
            else
            {
                ing.ID = -1;
            }
            con.Close();
            return ing;
        }

        public static List<Ingredient> GetAllIngredients()
        {
            List<Ingredient> ing_list = new List<Ingredient>();
            Ingredient temp;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Ingredient";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                temp = new Ingredient();
                temp.ID = int.Parse(dr["ID"].ToString());
                temp.Name = dr["Name"].ToString();
                temp.Comment = dr["Comment"].ToString();
                ing_list.Add(temp);
            }
            con.Close();
            return ing_list;
        }

        public static bool UpdateIngredient(Ingredient ing)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Ingredient SET Name = @name, Comment = @comment WHERE ID = @id";
            cm.Parameters.AddWithValue("@name", ing.Name);
            cm.Parameters.AddWithValue("@comment", ing.Comment);
            cm.Parameters.AddWithValue("@id", ing.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //public static List<int> GetProceduresForIngredient(Ingredient ing)
        //{
        //    List<int> proc_id_list = new List<int>();
        //    SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
        //    SqlCeCommand cm = con.CreateCommand();
        //    SqlCeDataReader dr = null;
        //    cm.CommandText = "SELECT * FROM Products WHERE Ingredient_ID = @id";
        //    cm.Parameters.AddWithValue("@id", ing.ID);
        //    con.Open();
        //    dr = cm.ExecuteReader();
        //    while (dr.Read())
        //    {
        //        proc_id_list.Add(int.Parse(dr["Procedure_ID"].ToString()));
        //    }
        //    return proc_id_list;
        //}

        public static bool AddNewIngredient(Ingredient ing)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Ingredient (Name, Comment) VALUES (@name, @comment)";
            cm.Parameters.AddWithValue("@name", ing.Name);
            cm.Parameters.AddWithValue("@comment", ing.Comment);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Procedure Functions
        //
        
        public static Procedure GetProcedure(int ID)
        {
            Procedure pro = new Procedure();
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Procedures WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", ID);
            con.Open();
            dr = cm.ExecuteReader();
            if (dr.Read())
            {
                pro.ID = int.Parse(dr["ID"].ToString());
                pro.Name = dr["Name"].ToString();
                pro.Comment = dr["Comment"].ToString();
            }
            else
            {
                pro.ID = -1;
            }
            con.Close();
            return pro;
        }

        public static List<Procedure> GetAllProcedures()
        {
            List<Procedure> pro_list = new List<Procedure>();
            Procedure temp;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Procedures";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                temp = new Procedure();
                temp.ID = int.Parse(dr["ID"].ToString());
                temp.Name = dr["Name"].ToString();
                temp.Comment = dr["Comment"].ToString();
                pro_list.Add(temp);
            }
            con.Close();
            return pro_list;
        }

        public static bool UpdateProcedure(Procedure pro)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Procedures SET Name = @name, Comment = @comment WHERE ID = @id";
            cm.Parameters.AddWithValue("@name", pro.Name);
            cm.Parameters.AddWithValue("@comment", pro.Comment);
            cm.Parameters.AddWithValue("@id", pro.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool AddNewProcedure(Procedure pro)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Procedures (Name, Comment) VALUES (@name, @comment)";
            cm.Parameters.AddWithValue("@name", pro.Name);
            cm.Parameters.AddWithValue("@comment", pro.Comment);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Effects Functions
        //
        public static Effect GetEffect(int ID)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            Effect e = new Effect();
            cm.CommandText = "SELECT * FROM Effects WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", ID);
            con.Open();
            dr = cm.ExecuteReader();
            if (dr.Read())
            {
                e.ID = int.Parse(dr["ID"].ToString());
                e.Name = dr["Name"].ToString();
                e.Power = int.Parse(dr["Power"].ToString());
                e.Commulative = bool.Parse(dr["Commulative"].ToString());
                e.Description = dr["Description"].ToString();
                e.Comment = dr["Comment"].ToString();
                if (bool.Parse(dr["Has_Counter"].ToString()))
                {
                    e.Counters_IDs = Get_Counters(e.ID);
                }
            }
            con.Close();
            return e;
        }

        public static List<Effect> Get_Effects(List<int> effect_ids)
        {
            bool has_counters;
            string id_list = IDListToString(effect_ids);
            List<Effect> effects = new List<Effect>();
            Effect e = null;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Effects WHERE ID IN(" + id_list + ")";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                e = new Effect();
                e.ID = int.Parse(dr["ID"].ToString());
                e.Name = dr["Name"].ToString();
                e.Power = int.Parse(dr["Power"].ToString());
                e.Commulative = bool.Parse(dr["Commulative"].ToString());
                e.Description = dr["Description"].ToString();
                e.Comment = dr["Comment"].ToString();
                has_counters = bool.Parse(dr["Has_Counter"].ToString());
                if (has_counters)
                {
                    e.Counters_IDs = Get_Counters(e.ID);
                }
                effects.Add(e);
            }
            con.Close();
            return effects;
        }

        public static List<Effect> GetAllEffects()
        {
            List<Effect> eff_list = new List<Effect>();
            Effect temp;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Effects";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                temp = new Effect();
                temp.ID = int.Parse(dr["ID"].ToString());
                temp.Name = dr["Name"].ToString();
                temp.Power = int.Parse(dr["Power"].ToString());
                temp.Commulative = bool.Parse(dr["Commulative"].ToString());
                temp.Description = dr["Description"].ToString();
                temp.Comment = dr["Comment"].ToString();
                if (bool.Parse(dr["Has_Counter"].ToString()))
                {
                    temp.Counters_IDs = Get_Counters(temp.ID);
                }
                eff_list.Add(temp);
            }
            con.Close();
            return eff_list;
        }

        public static bool UpdateEffect(Effect e)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Effects SET Name = @name, Power = @power, Commulative = @commulative, Has_Counter = @has_counter, Description = @description, Comment = @comment WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", e.ID);
            cm.Parameters.AddWithValue("@name", e.Name);
            cm.Parameters.AddWithValue("@power", e.Power);
            cm.Parameters.AddWithValue("@commulative", e.Commulative);
            cm.Parameters.AddWithValue("@has_counter", e.Counters_IDs.Any());
            cm.Parameters.AddWithValue("@description", e.Description);
            cm.Parameters.AddWithValue("@comment", e.Comment);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool AddNewEffect(Effect e)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Effects (Name, Power, Commulative, Has_Counter, Description, Comment) VALUES (@name, @power, @commulative, @has_counter, @description, @comment)";
            cm.Parameters.AddWithValue("@name", e.Name);
            cm.Parameters.AddWithValue("@power", e.Power);
            cm.Parameters.AddWithValue("@commulative", e.Commulative);
            cm.Parameters.AddWithValue("@has_counter", e.Counters_IDs.Any());
            cm.Parameters.AddWithValue("@description", e.Description);
            cm.Parameters.AddWithValue("@comment", e.Comment);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Counters Functions
        //
        public static List<int> Get_Counters(int effect_id)
        {
            List<int> ID_List = new List<int>();
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT Counter_ID FROM Counters WHERE Effect_ID = " + effect_id.ToString();
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                ID_List.Add(int.Parse(dr["Counter_ID"].ToString()));
            }
            con.Close();
            return ID_List;
        }

        public static bool RemoveEffectFromCounters(Effect origin_effect, Effect removed_effect)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "DELETE FROM Counters WHERE Effect_ID = @OID AND Counter_ID = @RID";
            cm.Parameters.AddWithValue("@OID", origin_effect.ID);
            cm.Parameters.AddWithValue("@RID", removed_effect.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool AddEffectToCounters(Effect origin_effect, Effect added_effect)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Counters (Effect_ID, Counter_ID) Values (@OID, @AID)";
            cm.Parameters.AddWithValue("@OID", origin_effect.ID);
            cm.Parameters.AddWithValue("@AID", added_effect.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Product Functions
        //

        public static List<Product> GetAllProducts()
        {
            List<Product> prod_list = new List<Product>();
            Product prod;
            List<int> effect_id_list;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Products";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                prod = new Product();
                prod.ID = int.Parse(dr["ID"].ToString());
                prod.Main_Effect_ID = int.Parse(dr["Main_Effect_ID"].ToString());
                prod.Product_Ingredient = GetIngredient(int.Parse(dr["Ingredient_ID"].ToString()));
                prod.Product_Procedure = GetProcedure(int.Parse(dr["Procedure_ID"].ToString()));
                prod.Comment = dr["Comment"].ToString();
                effect_id_list = GetProductEffectIDs(prod.ID);
                if (effect_id_list.Any())
                {
                    prod.Effects = Get_Effects(effect_id_list);
                    foreach (Effect e in prod.Effects)
                        e.Owner = prod;
                }
                prod_list.Add(prod);
            }
            con.Close();
            return prod_list;
        }

        //Returns the product if exists or an empty product with ID of -1 if it doesn't.
        public static Product GetProduct(Ingredient ing, Procedure pro)
        {
            Product prod = new Product();
            List<int> effect_id_list;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Products WHERE Ingredient_ID = @ing_ID AND Procedure_ID = @proc_ID";
            cm.Parameters.AddWithValue("@ing_ID", ing.ID);
            cm.Parameters.AddWithValue("@proc_ID", pro.ID);
            con.Open();
            dr = cm.ExecuteReader();
            if (dr.Read())
            {
                prod.ID = int.Parse(dr["ID"].ToString());
                prod.Main_Effect_ID = int.Parse(dr["Main_Effect_ID"].ToString());
                prod.Product_Ingredient = GetIngredient(int.Parse(dr["Ingredient_ID"].ToString()));
                prod.Product_Procedure = GetProcedure(int.Parse(dr["Procedure_ID"].ToString()));
                prod.Comment = dr["Comment"].ToString();
                effect_id_list = GetProductEffectIDs(prod.ID);
                if (effect_id_list.Any())
                {
                    prod.Effects = Get_Effects(effect_id_list);
                    foreach (Effect e in prod.Effects)
                        e.Owner = prod;
                }
            }
            else
            {
                prod.Product_Ingredient = ing;
                prod.Product_Procedure = pro;
            }
            con.Close();
            return prod;
        }

        public static List<int> GetProductEffectIDs(int ID)
        {
            List<int> id_list = new List<int>();
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Products_To_Effects WHERE Product_ID = @id";
            cm.Parameters.AddWithValue("@id", ID);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                id_list.Add(int.Parse(dr["Effect_ID"].ToString()));
            }
            con.Close();
            return id_list;
        }

        public static bool AddEffectToProduct(Effect effect, Product product)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Products_To_Effects (Product_ID, Effect_ID) Values (@PID, @EID)";
            cm.Parameters.AddWithValue("@PID", product.ID);
            cm.Parameters.AddWithValue("@EID", effect.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool RemoveEffectFromProduct(Effect effect, Product product)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "DELETE FROM Products_To_Effects WHERE Product_ID = @PID AND Effect_ID = @EID";
            cm.Parameters.AddWithValue("@PID", product.ID);
            cm.Parameters.AddWithValue("@EID", effect.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //Does not update the effects in DB, use Add/Remove Effect from Product instead
        public static bool UpdateProduct(Product p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Products SET Comment = @comment, Main_Effect_ID = @main_id WHERE ID = @id";
            cm.Parameters.AddWithValue("@main_id", p.Main_Effect_ID);
            cm.Parameters.AddWithValue("@comment", p.Comment);
            cm.Parameters.AddWithValue("@id", p.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }
        
        //Add the new Product and but not the effects in it.
        public static bool AddNewProduct(Product p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Products (Ingredient_ID, Procedure_ID, Comment, Main_Effect_ID) VALUES (@ing_ID, @pro_ID, @comment, @main_ID)";
            cm.Parameters.AddWithValue("@ing_ID", p.Product_Ingredient.ID);
            cm.Parameters.AddWithValue("@pro_ID", p.Product_Procedure.ID);
            cm.Parameters.AddWithValue("@comment", p.Comment);
            cm.Parameters.AddWithValue("@main_ID", p.Main_Effect_ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Specials Functions
        //
        public static List<Special> GetAllSpecials()
        {
            List<Special> spec_list = new List<Special>();
            Special temp;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Specials";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                temp = new Special();
                temp.ID = int.Parse(dr["ID"].ToString());
                temp.Name = dr["Name"].ToString();
                temp.Power = int.Parse(dr["Power"].ToString());
                temp.Comment = dr["Comment"].ToString();
                temp.Effect_Pairs = GetSpecialEffectPairs(temp.ID);
                if (temp.Effect_Pairs.Any())
                    foreach (KeyValuePair<Effect, Effect> item in temp.Effect_Pairs)
                    {
                        item.Key.Owner = temp;
                        item.Value.Owner = temp;
                    }
                spec_list.Add(temp);
            }
            con.Close();
            return spec_list;
        }

        public static Special GetSpecial(int ID)
        {
            Special spec = new Special();
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Specials WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", ID);
            con.Open();
            dr = cm.ExecuteReader();
            if (dr.Read())
            {
                spec.ID = int.Parse(dr["ID"].ToString());
                spec.Power = int.Parse(dr["Power"].ToString());
                spec.Comment = dr["Comment"].ToString();
                spec.Effect_Pairs = GetSpecialEffectPairs(spec.ID);
                if (spec.Effect_Pairs.Any())
                    foreach (KeyValuePair<Effect, Effect> item in spec.Effect_Pairs)
                    {
                        item.Key.Owner = spec;
                        item.Value.Owner = spec;
                    }
                
            }
            con.Close();
            return spec;
        }

        public static Dictionary<Effect, Effect> GetSpecialEffectPairs(int Spec_ID)
        {
            Dictionary<Effect, Effect> effect_pairs = new Dictionary<Effect, Effect>();
            List<Effect> keylist, vallist;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Specials_To_Effects WHERE Special_ID = @SID";
            cm.Parameters.AddWithValue("@SID", Spec_ID);
            con.Open();
            dr = cm.ExecuteReader();
            Dictionary<int, int> pairs_ids = new Dictionary<int,int>();
            while (dr.Read())
            {
                pairs_ids.Add(int.Parse((dr["Old_ID"].ToString())), int.Parse((dr["New_ID"].ToString())));
            }
            if (pairs_ids.Any())
            {
                keylist = Get_Effects(pairs_ids.Keys.ToList());
                vallist = Get_Effects(pairs_ids.Values.ToList());
                foreach (KeyValuePair<int,int> pair in pairs_ids)
                {
                    effect_pairs.Add(keylist.Find(item => item.ID == pair.Key), vallist.Find(item => item.ID == pair.Value));
                }
            }
            con.Close();
            return effect_pairs;
        }

        public static bool AddEffectPairToSpecial(Effect orig, Effect dest, Special spec)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Specials_To_Effects (Special_ID, Old_ID, New_ID) Values (@SID, @OID, @NID)";
            cm.Parameters.AddWithValue("@SID", spec.ID);
            cm.Parameters.AddWithValue("@OID", orig.ID);
            cm.Parameters.AddWithValue("@NID", dest.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool RemovePairFromSpecial(Effect orig, Special spec)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "DELETE FROM Specials_To_Effects WHERE Special_ID = @SID AND Old_ID = @OID";
            cm.Parameters.AddWithValue("@SID", spec.ID);
            cm.Parameters.AddWithValue("@OID", orig.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //Does not update the effects in DB, use Add/Remove Pair from Special instead
        public static bool UpdateSpecial(Special spec)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Specials SET Name = @name, Power = @power, Comment = @comment WHERE ID = @id";
            cm.Parameters.AddWithValue("@name", spec.Name);
            cm.Parameters.AddWithValue("@power", spec.Power);
            cm.Parameters.AddWithValue("@comment", spec.Comment);
            cm.Parameters.AddWithValue("@id", spec.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //Add the new Special but not the effects in it.
        public static bool AddNewSpecial(Special spec)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Specials (Name, Power, Comment) VALUES (@name, @power, @comment)";
            cm.Parameters.AddWithValue("@name", spec.Name);
            cm.Parameters.AddWithValue("@power", spec.Power);
            cm.Parameters.AddWithValue("@comment", spec.Comment);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        //
        //Potion Functions
        //

        public static List<Potion> GetAllPotions()
        {
            List<Potion> pot_list = new List<Potion>();
            Potion pot;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Potions";
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                pot = new Potion();
                pot.ID = int.Parse(dr["ID"].ToString());
                pot.Name = dr["Name"].ToString();
                pot.Description = dr["Description"].ToString();
                pot.Creator = dr["Creator_Name"].ToString();
                pot.Effects = GetPotionEffects(pot.ID);
                foreach (FinalEffect e in pot.Effects)
                    e.Owner = pot;
                pot_list.Add(pot);
            }
            con.Close();
            return pot_list;
        }

        public static List<FinalEffect> GetPotionEffects(int ID)
        {
            List<FinalEffect> effect_list = new List<FinalEffect>();
            FinalEffect fe;
            Effect e;
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            SqlCeDataReader dr = null;
            cm.CommandText = "SELECT * FROM Potion_To_Effect WHERE (Potion_ID = @PID)";
            cm.Parameters.AddWithValue("@PID", ID);
            con.Open();
            dr = cm.ExecuteReader();
            while (dr.Read())
            {
                e = GetEffect(int.Parse(dr["Effect_ID"].ToString()));
                fe = EffectToFinalEffect(e);
                fe.Level = int.Parse(dr["Effect_Level"].ToString());
                fe.InPotion = true;
                effect_list.Add(fe);
            }
            con.Close();
            return effect_list;
        }

        public static FinalEffect EffectToFinalEffect(Effect e)
        {
            FinalEffect fe = new FinalEffect();
            fe.ID = e.ID;
            fe.Comment = e.Comment;
            fe.Commulative = e.Commulative;
            fe.Counters_IDs = e.Counters_IDs;
            fe.Description = e.Description;
            fe.Name = e.Name;
            fe.Power = e.Power;
            return fe;
        }

        public static bool AddNewPotion(Potion p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Potions (Name, Description, Creator_Name) VALUES (@name, @desc, @creator)";
            cm.Parameters.AddWithValue("@name", p.Name);
            cm.Parameters.AddWithValue("@desc", p.Description);
            cm.Parameters.AddWithValue("@creator", p.Creator);
            con.Open();
            cm.ExecuteNonQuery();
            //Get ID
            cm = con.CreateCommand();
            cm.CommandText = "SELECT ID FROM Potions WHERE ID = @@IDENTITY";
            p.ID = (int)cm.ExecuteScalar();
            con.Close();
            foreach (FinalEffect fe in p.Effects)
                AddEffectToPotion(fe, p);
            return true;
        }

        public static bool AddEffectToPotion(FinalEffect fe, Potion p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "INSERT INTO Potion_To_Effect (Potion_ID, Effect_ID, Effect_Level) Values (@PID, @EID, @ELVL)";
            cm.Parameters.AddWithValue("@PID", p.ID);
            cm.Parameters.AddWithValue("@EID", fe.ID);
            cm.Parameters.AddWithValue("@ELVL", fe.Level);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool UpdatePotion(Potion p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "UPDATE Potions SET Name = @name, Description = @desc, Creator_Name = @creator WHERE ID = @id";
            cm.Parameters.AddWithValue("@name", p.Name);
            cm.Parameters.AddWithValue("@desc", p.Description);
            cm.Parameters.AddWithValue("@creator", p.Creator);
            cm.Parameters.AddWithValue("@id", p.ID);
            con.Open();
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }

        public static bool DeletePotion(Potion p)
        {
            SqlCeConnection con = new SqlCeConnection(DB_ADDRESS);
            SqlCeCommand cm = con.CreateCommand();
            cm.CommandText = "DELETE FROM Potions WHERE ID = @id";
            cm.Parameters.AddWithValue("@id", p.ID);
            con.Open();
            cm.ExecuteNonQuery();
            //Remove from Potion_To_Effect as well
            cm = con.CreateCommand();
            cm.CommandText = "DELETE FROM Potion_To_Effect WHERE Potion_ID = @id";
            cm.Parameters.AddWithValue("@id", p.ID);
            cm.ExecuteNonQuery();
            con.Close();
            return true;
        }
    }
}
