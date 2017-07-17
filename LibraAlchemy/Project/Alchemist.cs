using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LibraAlchemy
{
    class Alchemist
    {
        public List<Product> Product_List { get; set; }
        public Potion Current_Potion { get; set; }
        public List<Ingredient> Ingredient_List { get; set; }
        public List<Procedure> Procedure_List { get; set; }
        public List<Effect> Effect_List { get; set; }
        public List<Special> Special_List { get; set; }
        public List<Potion> Potion_List { get; set; }

        public Alchemist()
        {
            Product_List = new List<Product>();
            Current_Potion = new Potion();
            Ingredient_List = new List<Ingredient>();
            Procedure_List = new List<Procedure>();
            Effect_List = new List<Effect>();
            Special_List = new List<Special>();
        }

        //
        //Craft Potion Functions
        //

        public void MakePotion(List<Object> inPotionList)
        {
            List<Effect> effectPool = new List<Effect>();
            //Divide the object list into products and specials, but we still reference the same item.
            List<Product> prodsInPotion = new List<Product>();
            List<Special> specsInPotion = new List<Special>();
            foreach (Object obj in inPotionList)
            {
                if (obj is Product)
                    prodsInPotion.Add(obj as Product);
                if (obj is Special)
                    specsInPotion.Add(obj as Special);
            }
            //If we have no products there is no potion
            if (!prodsInPotion.Any())
                return;
            //Now we will check the products. We will check for doubles first.
            CheckDoubles(prodsInPotion, effectPool);
            //Before we apply the counters we need to sort the effects by their power.
            effectPool.Sort((x, y) => x.Power.CompareTo(y.Power));
            //Now we will check the counters and mark the relevant effects as Counterd and not in potion, removing them from the effectPool as well.
            ApplyCounters(effectPool);
            //We will now count the effects remaining, determining their level (if they are commulative) and if they are in the potion or not.
            //FinalEffect is a class which simply has a few owners rather than one (for every appearance of the effect), also - this is the class that is altered by
            //specials.
            List<FinalEffect> final_effects = CheckEffectsStrength(effectPool);
            //Now if we have Specials, we will apply them now to the final effects (NOT THE EFFECTS IN THE POTION POOL).
            if (specsInPotion.Any())
                ApplySpecials(final_effects, specsInPotion);
            //sort final effects by power to determine the potion final effects
            Current_Potion = FinallizePotion(final_effects);
        }

        public void CheckDoubles(List<Product> prodsInPotion, List<Effect> effectPool)
        {
            List<Product> temp_list;
            //Create a distinct list of Product IDs
            List<int> dist_prod_ids = prodsInPotion.Select(p => p.ID).Distinct().ToList();
            //If we find doubles, we will mark their non-main effects as Not in potion and give them Grey Color.
            foreach (int ID in dist_prod_ids)
            {
                temp_list = new List<Product>();
                temp_list = prodsInPotion.FindAll(item => item.ID == ID);
                //If count is above one that means we have duplicates!
                if (temp_list.Count > 1)
                {
                    foreach (Product prod in temp_list)
                    {
                        foreach (Effect e in prod.Effects)
                        {
                            if (e.ID != prod.Main_Effect_ID)
                            {
                                e.InPotion = false;
                                e.FontColor = System.Drawing.Color.Gray;
                            }
                            else
                            {
                                effectPool.Add(e);
                            }
                        }
                    }
                }
                else
                {
                    //Not a duplicate - Insert all of the effects into the pool
                    foreach (Effect e in temp_list[0].Effects)
                    {
                        effectPool.Add(e);
                    }
                }
            }
        }

        //Takes a list of effects and apply all the counters in them, removing the effects from pot_effects afterwards.
        private void ApplyCounters(List<Effect> pot_effects)
        {
            int counter_index = -1;
            List<Effect> effects;
            foreach (Effect e in pot_effects)
            {
                if (e.Counters_IDs.Count == 0 || !(e.InPotion)) //No counters for this effect or this effect is already countered.
                    continue;
                else //This means the effect does have counters and that it is in the potion.
                {
                    //Search for a counter for each in the list. the first ID is the priority counter so first we have to select the counters from the DB and sort the
                    //Counters_IDs by Ascending Order, because the powerful effect (with lower value of power) is countered first.
                    effects = DAL.Get_Effects(e.Counters_IDs);
                    //Sort them ascending by power
                    effects.Sort((x, y) => x.Power.CompareTo(y.Power));
                    e.Counters_IDs = effects.Select(item => item.ID).ToList();
                    //Now that they are sorted by power, we can begin the iteration..
                    foreach (int ID in e.Counters_IDs)
                    {
                        //Try and find the counter somewhere in the potion effects pool
                        counter_index = pot_effects.FindIndex(item => item.ID == ID && item.InPotion);
                        if (counter_index >= 0)
                        {
                            //Mark both of the effects as not in potion
                            e.InPotion = false;
                            pot_effects[counter_index].InPotion = false;
                            e.IsCountered = true;
                            pot_effects[counter_index].IsCountered = true;
                            e.FontColor = System.Drawing.Color.Red;
                            pot_effects[counter_index].FontColor = System.Drawing.Color.Red;
                            e.CounterName = pot_effects[counter_index].Name;
                            pot_effects[counter_index].CounterName = e.Name;
                            break;
                        }
                    }
                }
            }
            //Remove all the countered effects from the pool
            pot_effects.RemoveAll(item => item.InPotion == false);
        }

        //Checks if effects are Main or secondary effects of potion (removing them from pot effects if secondary), also calculates level of commulative effects.
        private List<FinalEffect> CheckEffectsStrength(List<Effect> pot_effects)
        {
            List<Effect> results;
            List<FinalEffect> final_effects = new List<FinalEffect>();
            FinalEffect final;
            //Put the effects in a dictionary of (Effect.ID, Count of times it appeared)
            Dictionary<int, int> counts = pot_effects.GroupBy(x => x.ID)
                                      .ToDictionary(g => g.Key,
                                                    g => g.Count());
            foreach (KeyValuePair<int, int> entry in counts)
            {
                //Get the list of all the effects that match this entry:
                results = pot_effects.FindAll(item => item.ID == entry.Key);
                //Check if the entry's strength is 1 (or less, shouldn't be less)
                if (entry.Value == 0)
                {
                    //Shouldn't happen
                }
                if (entry.Value == 1)
                {
                    //Not strong enough to be in potion
                    results[0].InPotion = false;
                }
                //Else the entry has more than one appearance which means it is in the potion. If it is commulative the level is equal to the entry value.
                else
                {
                    //Create the FinalEffect to enter the list of the effects of the potion.
                    final = new FinalEffect();
                    final.Commulative = results[0].Commulative;
                    final.Counters_IDs = results[0].Counters_IDs;
                    final.ID = results[0].ID;
                    final.InPotion = true;
                    if (final.Commulative)
                        final.Level = entry.Value - 1; //Because 2 apperances are equal to level 1 and so forth..
                    else
                        final.Level = 1;
                    final.Name = results[0].Name;
                    final.Power = results[0].Power;
                    final.Description = results[0].Description;
                    final.Parents = new List<Effect>();
                    foreach (Effect e in results)
                    {
                        e.InPotion = true;
                        e.FontColor = System.Drawing.Color.Green;
                        if (e.Commulative)
                            e.Level = entry.Value;
                        final.Parents.Add(e);
                    }
                    final_effects.Add(final);
                }
            }
            pot_effects.RemoveAll(item => item.InPotion == false);
            return final_effects;
        }

        private void ApplySpecials(List<FinalEffect> final_effects, List<Special> spec_list)
        {
            int index;
            //Sort the special list by power so the strongest Special will apply first, changing the effect completley and not allowing other specials to alter it.
            spec_list.Sort((x, y) => x.Power.CompareTo(y.Power));
            foreach (Special spec in spec_list)
            {
                foreach (KeyValuePair<Effect, Effect> entry in spec.Effect_Pairs)
                {
                    //Try to find the index of the original effect that we want to alter
                    index = final_effects.FindIndex(item => item.ID == entry.Key.ID);
                    if (index < 0)
                    {
                        continue;
                    }
                    //This means that one of the final effect is indeed the ID we were looking for
                    else
                    {
                        //Every effect can only be changed once by a Special.
                        if (final_effects[index].ChangedBySpecial)
                            continue;
                        //Change the Final Effect according to the special. Key is the old effect, Value is the new one.
                        final_effects[index].Changer = spec;
                        final_effects[index].ChangedBySpecial = true;
                        final_effects[index].OldName = entry.Key.Name;
                        final_effects[index].Commulative = entry.Value.Commulative;
                        if (!final_effects[index].Commulative)
                            final_effects[index].Level = 1;
                        else
                            final_effects[index].Level = final_effects[index].Parents[0].Level;
                        final_effects[index].ID = entry.Value.ID;
                        final_effects[index].Name = entry.Value.Name;
                        final_effects[index].Power = entry.Value.Power;
                        final_effects[index].Description = entry.Value.Description;
                        final_effects[index].Comment = entry.Value.Comment;
                        final_effects[index].FontColor = System.Drawing.Color.Gold;
                    }
                }
            }
            //The special case of two FinalEffects of the same kind, need to merge them into one.
            List<FinalEffect> dups, toRemove = new List<FinalEffect>();
            FinalEffect remainder;
            List<int> duplicatedeffectsIDs = (from e in final_effects
                                    group e by e.ID into g
                                    where g.Count() > 1
                                    select g.Key).ToList();
            foreach (int ID in duplicatedeffectsIDs)
            {
                dups = final_effects.FindAll(item => item.ID == ID);
                remainder = dups[0];
                for (int i = 1; i < dups.Count; i++)
                {
                    if (remainder.Commulative)
                        remainder.Level += dups[i].Level;
                    if (dups[i].ChangedBySpecial)
                    {
                        remainder.ChangedBySpecial = true;
                        remainder.Changer = dups[i].Changer;
                        remainder.OldName = dups[i].OldName;
                        remainder.FontColor = System.Drawing.Color.Gold;
                    }
                    remainder.Parents.AddRange(dups[i].Parents);
                    toRemove.Add(dups[i]);
                }
            }
            foreach (FinalEffect fe in toRemove)
                final_effects.Remove(fe);

        }

        private Potion FinallizePotion(List<FinalEffect> final_effects)
        {
            Potion pot = new Potion();
            //Sort Ascending
            final_effects.Sort((x, y) => x.Power.CompareTo(y.Power));
            pot.Effects = final_effects;
            return pot;
        }

        //
        //Ingredient Functions
        //

        public void GetAllIngredients()
        {
            Ingredient_List = DAL.GetAllIngredients();
            Ingredient_List.Sort(new IngredientStringComparer());
        }

        public bool UpdateIngredient(Ingredient ing)
        {
            //If the ingredient has a positive ID it means it already exists but needs to be altered, otherwise create a new one.
            //Update
            if (ing.ID > 0)
            {
                return DAL.UpdateIngredient(ing);
            }
            else
            //Add New
            {
                return DAL.AddNewIngredient(ing);
            }
        }

        //
        //Procedures Functions
        //

        public void GetAllProcedures()
        {
            Procedure_List = DAL.GetAllProcedures();
            Procedure_List.Sort(new ProcedureStringComparer());
        }

        public bool UpdateProcedure(Procedure pro)
        {
            //If the Procedure has a positive ID it means it already exists but needs to be altered, otherwise create a new one.
            //Update
            if (pro.ID > 0)
            {
                return DAL.UpdateProcedure(pro);
            }
            else
            //Add New
            {
                return DAL.AddNewProcedure(pro);
            }
        }

        //
        //Effect Functions
        //

        public void GetAllEffects()
        {
            Effect_List = DAL.GetAllEffects();
            Effect_List.Sort(new EffectStringComparer());
        }

        public void UpdateEffect(Effect e)
        {
            DAL.UpdateEffect(e);
        }

        public void AddNewEffect(Effect e)
        {
            int max = 0;
            //If empty, try to get the from DB
            if (!Effect_List.Any())
                GetAllEffects();
            if (Effect_List.Any())
                max = Effect_List.OrderByDescending(item => item.Power).First().Power;
            //if the last statement was false, the DB is empty and this is the first Effect, which will start with Power of 1.
            //The new effect would have the highest power - which makes it the weakest.
            e.Power = max + 1;
            DAL.AddNewEffect(e);
        }

        public void RemoveCounterFromEffect(Effect origin_effect, Effect removed_effect)
        {
            //Remove the effect from the counter list of the original effect
            origin_effect.Counters_IDs.RemoveAll(item => item == removed_effect.ID);
            removed_effect.Counters_IDs.RemoveAll(item => item == origin_effect.ID);
            if (!origin_effect.Counters_IDs.Any())
            {
                //If the effect doesn't have counters anymore, the database should know...
                DAL.UpdateEffect(origin_effect);
            }
            if (!removed_effect.Counters_IDs.Any())
            {
                DAL.UpdateEffect(removed_effect);
            }
            //Now remove the effect from the Counters table...
            DAL.RemoveEffectFromCounters(origin_effect, removed_effect);
            DAL.RemoveEffectFromCounters(removed_effect, origin_effect);
        }

        public void AddCounterToEffect(Effect origin_effect, Effect added_effect)
        {
            origin_effect.Counters_IDs.Add(added_effect.ID);
            added_effect.Counters_IDs.Add(origin_effect.ID);
            if (origin_effect.Counters_IDs.Count == 1)
            {
                //This means the effect had 0 counters before, the DB should know it has counters now!
                DAL.UpdateEffect(origin_effect);
            }
            if (added_effect.Counters_IDs.Count == 1)
            {
                DAL.UpdateEffect(added_effect);
            }
            DAL.AddEffectToCounters(origin_effect, added_effect);
            DAL.AddEffectToCounters(added_effect, origin_effect);
        }

        //
        //Product Functions
        //
        public void GetAllProducts()
        {
            Product_List = DAL.GetAllProducts();
        }

        public Product GetProduct(Ingredient ing, Procedure proc)
        {
            return DAL.GetProduct(ing, proc);
        }

        public void RemoveEffectFromProduct(Effect e, Product p)
        {
            if (p.Main_Effect_ID == e.ID)
            {
                Effect nextInLine = p.Effects.Find(item => item.ID != e.ID);
                if (nextInLine != null)
                {
                    //Put the next in line as the main effect
                    p.Main_Effect_ID = nextInLine.ID;
                }
                else
                {
                    p.Main_Effect_ID = -1;
                }
                DAL.UpdateProduct(p);
            }
            DAL.RemoveEffectFromProduct(e, p);
        }

        public void AddEffectToProduct(Effect e, Product p)
        {
            DAL.AddEffectToProduct(e, p);
            if (p.Main_Effect_ID == -1)
            {
                p.Main_Effect_ID = e.ID;
                DAL.UpdateProduct(p);
            }
        }

        public void AddNewProduct(Product p)
        {
            DAL.AddNewProduct(p);
        }

        public void UpdateProduct(Product p)
        {
            DAL.UpdateProduct(p);
        }

        public void RefreshProduct(Product prod)
        {
            foreach (Effect e in prod.Effects)
            {
                e.InPotion = true;
                e.IsCountered = false;
                e.Level = 1;
                e.FontColor = System.Drawing.Color.Black;
                e.CounterName = "";
            }
        }

        public List<Product> FindProductsWithEffects(List<Effect> effect_list)
        {
            List<Product> result = new List<Product>();
            foreach (Product p in Product_List)
            {
                if (!effect_list.Except(p.Effects, new EffectComparer()).Any())
                {
                    result.Add(p);
                }
            }
            return result;
        }
        //
        //Specials Functions
        //

        public void GetAllSpecials()
        {
            Special_List = DAL.GetAllSpecials();
            Special_List.Sort(new SpecialStringComparer());
        }

        public void RemovePairFromSpecial(Effect orig, Special spec)
        {
            //Remove the orig and its pair from the spec object
            spec.Effect_Pairs.Remove(orig);
            //Remove it from the DB
            DAL.RemovePairFromSpecial(orig, spec);
        }

        public void AddPairToSpecial(Special spec, Effect orig, Effect dest)
        {
            //Add to DB
            DAL.AddEffectPairToSpecial(orig, dest, spec);
            //Add to the dictionary
            Effect oe, de;
            oe = DAL.GetEffect(orig.ID);
            de = DAL.GetEffect(dest.ID);
            oe.Owner = spec;
            de.Owner = spec;
            spec.Effect_Pairs.Add(oe, de);
        }

        public void AddNewSpecial(Special spec)
        {
            int max = 0;
            if (!Special_List.Any())
            {
                GetAllSpecials();
            }
            if (Special_List.Any())
            {
                max = Special_List.OrderByDescending(item => item.Power).First().Power;
            }
            spec.Power = max + 1;
            DAL.AddNewSpecial(spec);
        }

        public void UpdateSpecial(Special spec)
        {
            DAL.UpdateSpecial(spec);
        }

        //
        //Potion Functions
        //

        public void GetAllPotions()
        {
            Potion_List = DAL.GetAllPotions();
            Potion_List.Sort(new PotionNameComparer());
        }

        //Null if no potion found.
        public Potion GetPotionByEffects(List<FinalEffect> effect_list)
        {
            //If the list is empty, no potion will be selected
            if (!effect_list.Any())
                return null;
            List<Potion> matches = new List<Potion>();
            //Get only the first five or less.
            List<Effect> final_list = new List<Effect>();
            if (effect_list.Count > 5)
                for (int i = 0; i < 5; i++)
                    final_list.Add(effect_list[i]);
            else
                foreach (FinalEffect e in effect_list)
                    final_list.Add(e);
            //Now we will compare the five effects to all of the other potions. 
            GetAllPotions();
            //this is not the most effective way but it's easier since the DB won't have thousands of records.
            matches = Potion_List.FindAll(item => item.Effects.Select(e => e.ID).ToList().OrderBy(x => x).SequenceEqual(effect_list.Select(i => i.ID).ToList().OrderBy(x => x)));
            //But what about the levels?
            bool flag = true;
            Effect temp;
            foreach (Potion match in matches)
            {
                foreach (FinalEffect fe in match.Effects)
                {
                    temp = final_list.Find(item => item.ID == fe.ID);
                    if (temp != null && temp.Level == fe.Level)
                    {
                        continue;
                    }
                    else
                    {
                        flag = false;
                        break;
                    }
                }
                if (flag)
                    return match;
                flag = true;
            }
            return null;
        }

        public void AddNewPotion(Potion p)
        {
            if (!p.Effects.Any())
                return;
            //Get only the first five or less.
            while (p.Effects.Count > 5)
                p.Effects.RemoveAt(5);
            //We don't have to check if it exists because otherwise we wouldn't be able to click the button
            DAL.AddNewPotion(p);
        }

        public void UpdatePotion(Potion p)
        {
            DAL.UpdatePotion(p);
        }

        public void DeletePotion(Potion p)
        {
            DAL.DeletePotion(p);
        }

        public List<Potion> FindPotionsWithEffects(List<Effect> effect_list)
        {
            List<Potion> result = new List<Potion>();
            foreach (Potion p in Potion_List)
            {
                if (!effect_list.Except(p.Effects, new EffectComparer()).Any())
                {
                    result.Add(p);
                }
            }
            return result;
        }

        public List<Potion> GetSimilarPotions(List<FinalEffect> effects)
        {
            //If the list is empty, no potion will be selected
            if (!effects.Any())
                return new List<Potion>();
            List<Potion> matches = new List<Potion>();
            //Get only the first five or less.
            List<Effect> final_list = new List<Effect>();
            if (effects.Count > 5)
                for (int i = 0; i < 5; i++)
                    final_list.Add(effects[i]);
            else
                foreach (FinalEffect e in effects)
                    final_list.Add(e);
            //Test pots is a temporary list of candidate potions
            List<Potion> test_pots;
            if (final_list.Count < 5)
            {
                test_pots = new List<Potion>();
                //We'll take all the potions that have 1 more effect than the effect list.
                test_pots = Potion_List.FindAll(item => item.Effects.Count == final_list.Count + 1);
                foreach (Potion pot in test_pots)
                {
                    //now we will see if any of them contains effect_list (and has effects of the same level).
                    if (!final_list.Except(pot.Effects, new EffectAndLevelComparer()).Any())
                    {
                        matches.Add(pot);
                    }
                }
            }
            if (final_list.Count > 1)
            {
                test_pots = new List<Potion>();
                //We'll take all the potions that have 1 less effect than the effect list. 
                test_pots = Potion_List.FindAll(item => item.Effects.Count == final_list.Count - 1);
                foreach (Potion pot in test_pots)
                {
                    //And now we will check if our list contains the potion effects - Get it?
                    if (!pot.Effects.Except(final_list, new EffectAndLevelComparer()).Any())
                    {
                        matches.Add(pot);
                    }
                }
            }
            test_pots = new List<Potion>();
            //Now we will take all the potions with the same amount of effects
            test_pots = Potion_List.FindAll(item => item.Effects.Count == final_list.Count);
            //We will remove each effect one at a time and check if the new list is contained in the list of test_pots. we will need a new temp_list for this.
            List<Effect> temp_list;
            for (int i = 0; i < final_list.Count; i++)
            {
                temp_list = new List<Effect>();
                temp_list.AddRange(final_list);
                temp_list.RemoveAt(i);
                //Now we will see if it is contained in any of the potions in test_pots. we may need to avoid duplicates in the end.
                foreach (Potion pot in test_pots)
                {
                    //now we will see if any of them contains effect_list (and has effects of the same level).
                    if (!temp_list.Except(pot.Effects, new EffectAndLevelComparer()).Any())
                    {
                        //They can't be EXACTLY the same. it's not similar!
                        if (!final_list.SequenceEqual(pot.Effects, new EffectAndLevelComparer()))
                            matches.Add(pot);
                    }
                }
            }
            //Now we will look for the potions that have exactly the same effects, but we will ignore levels.
            test_pots = new List<Potion>();
            //Get all of the same (not levelwise)
            test_pots = Potion_List.FindAll(item => item.Effects.SequenceEqual(final_list, new EffectComparer()));
            //Remove all of the same (levelwise)
            test_pots.RemoveAll(item => item.Effects.SequenceEqual(final_list, new EffectAndLevelComparer()));
            matches.AddRange(test_pots);
            return matches.Distinct().ToList();
        }


    }
}
