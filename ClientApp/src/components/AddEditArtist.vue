<template>
  <section id="app" class="section">
      <h1 class="title is-1">
          Add/Edit Artist
      </h1>

      <div class="columns">
          <div class="column">
              <form>
                  <div class="field">
                      <label class="label">Input Artist Name To Search</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.searchArtist" />
                      </div>
                      <div class="control">
                           <input class="button is-primary" type="submit" value="Get Artist" @click.prevent="getArtist"/>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Stream Type</label>
                      <div class="control">
                          <div class="select">
                              <select v-model="form.streamType">
                                  <option v-for="color in [{txt:'Pre', val: 0}, {txt:'Free', val: 1}, {txt:'Removed', val: 2}]" :key="color.val" :value="color.val">
                                      {{color.txt}}
                                  </option>
                              </select>
                          </div>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Account</label>
                      <div class="control">
                          <div class="select">
                              <select v-model="form.accountId">
                                  <option v-for="acc in accounts" :key="acc.id" :value="acc.id">
                                      {{acc.text}}
                                  </option>
                              </select>
                          </div>
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Artist Name</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.name" />
                      </div>
                  </div>

                  <div class="field">
                      <label class="label">Artist Id</label>
                      <div class="control margin-bottom">
                          <input class="input" type="text" v-model="form.spotifyId" />
                      </div>
                  </div>

                  <input class="button is-primary margin-bottom" type="submit" @click.prevent="update" value="Update"/>
                  <input class="button is-primary margin-bottom margin-left" type="submit" @click.prevent="add" value="Add"/>
              </form>

              <transition name="fade" mode="out-in">
                  <article class="message is-primary" v-show="showSubmitFeedback">
                      <div class="message-header">
                          <p>Send Status:</p>
                      </div>
                      <div class="message-body">
                          Successfully Submitted!
                      </div>
                  </article>
              </transition>
          </div>
          <div class="column">
              <h5>
                  JSON
              </h5>
              <pre><code>{{form}}</code></pre>
          </div>
      </div>

  </section>
</template>

<script>

import "vue-select/dist/vue-select.css";

export default {
  components: {  },
  data() {
    return {
      form: {
            searchArtist: "",
            name: "",
            spotifyId: "",
            accountId: 1,
            streamType: 0,
            id:0
        },
      showSubmitFeedback: false,
      accounts: [
        {id: 1, text: 'Adam'},
        {id: 2, text: 'Beck'},
        {id: 3, text: 'HH'},
        {id: 4, text: 'V'},
        {id: 5, text: 'Beck - Free'},
        {id: 6, text: 'Beck - Mix'},
      ],
    };
  },
  computed: {
    datasets() {
      if (!this.data) return null;
      return this.data.map((d) => {
        return {
          slug: d.account,
          data: d.streamInfos.map((i) => {
            return { value: i.streams, day: i.day };
          }),
        };
      });
    },
  },
  methods: {
    add() {
        this.form.id = 0;
        this.$http
        .post(`http://139.180.139.12/api/artist/create`, this.form)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
        }, 3000);
        })
        .catch((error) => console.log(error));
    },
    update() {
        this.$http
        .post(`http://139.180.139.12/api/artist/update`, this.form)
        .then((res) => {
          console.log(res.data);
          this.showSubmitFeedback = true;
          setTimeout(() => {
            this.showSubmitFeedback = false;
        }, 3000);
        })
        .catch((error) => console.log(error));
    },
    getArtist() {
      if(this.form.searchArtist == '') return;

      this.$http
        .get(`http://139.180.139.12/api/artist/name/` + this.form.searchArtist)
        .then((res) => {
          console.log(res.data);
          this.form.name = res.data.name;
          this.form.accountId = res.data.accountId;
          this.form.spotifyId = res.data.spotifyId;
          this.form.streamType = res.data.streamType;
          this.form.id = res.data.id;
        })
        .catch((error) => console.log(error));
    },
  },
  mounted() {
    
  },
};
</script>

<style lang="scss">
.margin-bottom {
  margin-bottom: 15px;
}

.margin-left {
  margin-left: 15px;
}

.fade-enter, .fade-leave-active {
  opacity: 0;
}

.fade-enter-active, .fade-leave-active {
  transition: opacity .5s;
}
</style>
