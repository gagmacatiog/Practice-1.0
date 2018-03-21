using Firebase.Database;
using Firebase.Database.Query;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ServicingTerminalApplication
{
    public class firebase_Connection
    {
        private const String databaseUrl = "https://usep-queue-app.firebaseio.com/";
        private const String databaseSecret = "xMkY2DmLnRefSl34kYlp8PWUzDwNmyJAvxLPygQ1";
        private int run = 0;
        //private const String node = "Queue_Info/";
        private FirebaseClient firebase;
        
        public firebase_Connection()
        {

            this.firebase = new FirebaseClient(
                databaseUrl,
                new FirebaseOptions
                {
                    AuthTokenAsyncFactory = () => Task.FromResult(databaseSecret)
                });

        }
        //public async Task TryFunction()
        //{
        //    await firebase.Child("first_var").PostAsync(new _Queue_Info());
        //    await firebase.Child("first_var").Child("2nd_var").PostAsync(new _Queue_Info());

        //    //await firebase.Child("Counter").;
        //    //await firebase.Child("Queue_Info").DeleteAsync();
        //}
        //public async Task Retrieve(_Queue_Info q_info)
        //{
        //    ref.child('videos/videoId1').orderByChild('time')
        //    await firebase.Child(node).OrderBy("ID").EqualTo(1).LimitToFirst(1).PatchAsync<_Queue_Info>(q_info);
        //    > await firebase.Child(node).OrderBy("ID").StartAt(5).EndAt(6).LimitToFirst(1).PatchAsync<_Queue_Info>(q_info);
        //    var cc = await firebase.Child(node).OrderBy("ID").LimitToFirst(2).OnceAsync<_Queue_Info>();
        //    foreach (var b in cc)
        //    {
        //        Console.WriteLine($"{b.Key} is {b.Object.ID}m high.");
        //    }
        //}
        public async Task App_Delete_MainQueue(int q_id)
        {
            string node = "Main_Queue/";
            Console.WriteLine("Deleting on -> Main_Queue");

            string key = "";
            var cc = await firebase.Child(node).OrderBy("ID").StartAt(q_id).EndAt((q_id)+1).LimitToFirst(1).OnceAsync<_Main_Queue>();
            foreach (var b in cc) { key = b.Key; }

            Console.WriteLine("App Delete MainQueue key - > " + key);

            try { await firebase.Child(node).Child(key).DeleteAsync(); }
            catch (Exception e) { Console.Write("Delete failed ! Error ->" + e); }
            finally { Console.Write("Delete finished."); }

        }
        public async Task App_Delete_QueueTransaction(string ID_Pattern) {
            string node = "Queue_Transaction/";
            Console.WriteLine("Deleting on -> Queue_Transacation");

            string key = "";
            var cc = await firebase.Child(node).OrderBy("ID_Pattern").StartAt(ID_Pattern).LimitToFirst(1).OnceAsync<_Queue_Transaction>();
            foreach (var b in cc) { key = b.Key; }

            Console.WriteLine("Key returned is " + key);

            try { await firebase.Child(node).Child(key).DeleteAsync(); }
            catch (Exception e) { Console.Write("Delete failed ! Error ->" + e); }
            finally { Console.Write("Delete finished."); }
        }
        public async Task App_Update_MainQueue(int where, _Main_Queue mq)
        {
            run++;
            string node = "Main_Queue/";
            Console.WriteLine("patch async App Update MainQueue on -> MainQueue"+run);
            Console.WriteLine("Received values are the following:"+mq.Queue_Number+" "+mq.Servicing_Office+" "+mq.Pattern_Current+" "+where);

            string key = "";
            var cc = await firebase.Child(node).OrderBy("ID").StartAt(where).EndAt((where + 1)).LimitToFirst(1).OnceAsync<_Main_Queue>();
            foreach (var b in cc) { key = b.Key;
                // Passing variables to the new _Main_Queue updated class
                mq.ID = b.Object.ID;
                mq.Transaction_Type = b.Object.Transaction_Type;
                mq.Student_No = b.Object.Student_No;
                mq.Customer_Queue_Number = b.Object.Customer_Queue_Number;
            }
            Console.WriteLine(run+" Query is > " + node+key+mq.ID);
            try { await firebase.Child(node).Child(key).PatchAsync(mq); }
            catch (Exception e) {   Console.Write("error ->" + e); }
            finally { Console.Write("Error happened: Update finished[?]"); }
            

        }
        public async Task InsertMultiple() {
            int x = 1;
            for (x = 1; x <= 10; x++) {
                _Queue_Info aa = new _Queue_Info{Servicing_Office = x};
                await firebase.Child("Queue_Info/").PostAsync<_Queue_Info>(aa);
            }

        }
        
        public async Task Delete(_Queue_Info q_info)
        {
            //>await firebase.Child(node).Child(q_info.Key).DeleteAsync();

        }
        public async void App_Update_QueueInfo(int where, _Queue_Info q_info)
        {
            string node = "Queue_Info/";
    Console.WriteLine("App Update Child running");

            string key = "";
            var cc = await firebase.Child(node).OrderBy("Servicing_Office").StartAt(where).EndAt((where + 1)).LimitToFirst(1).OnceAsync<_Queue_Info>();
            foreach (var b in cc) { key = b.Key; }

    Console.WriteLine("Key returned is " + key);

            try { await firebase.Child(node).Child(key).PatchAsync<_Queue_Info>(q_info); }
            catch(Exception e){ Console.Write("error ->" + e); }
            finally { Console.Write("Update finished."); }

        }
        //public async void Update(string key, _Queue_Info q_info)
        //{
        //    Console.WriteLine("Opening Update...");
        //    //>await firebase.Child(node).Child(key).PatchAsync<_Queue_Info>(q_info);
        //    //>Console.WriteLine(node + key);
        //}
        //public async Task Insert(_Queue_Info q_info)
        //{
        //    Console.Write("running...");
        //    //>await firebase.Child(node).PostAsync<_Queue_Info>(q_info);
        //    Console.Write("done?");
        //}

    }
}
