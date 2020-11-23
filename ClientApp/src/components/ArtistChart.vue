<template>
  <div class="single-artist">
    <div class="control margin-bottom">
        <div class="select">
            <select v-model="accountId" @change="onAccountChange()">
                <option disabled value="">Please select account</option>
                <option v-for="acc in accounts" :key="acc.id" :value="acc.id">
                    {{acc.name}}
                </option>
            </select>
        </div>
        <div class="select margin-left">
            <select v-model="selectMonth">
                <!-- <option disabled value="">Please select month</option> -->
                <option v-for="acc in months" :key="acc.id" :value="acc.id">
                    {{acc.text}}
                </option>
            </select>
        </div>
        <div class="select margin-left">
            <select v-model="selectYear">
                <!-- <option disabled value="">Please select year</option> -->
                <option v-for="acc in years" :key="acc.id" :value="acc.id">
                    {{acc.text}}
                </option>
            </select>
        </div>
    </div>
    <!-- <v-select v-model="selected" v-on:change="onChange($event)" 
        :options="options" :reduce="t => t.name" label="name" :value="selected" @input="onChange"
        placeholder="Please select an Artist">
    </v-select> -->
    <v-select v-model="selected"
        :options="options" :reduce="t => t.name" label="name" :value="selected"
        placeholder="Please select an Artist">
      <!-- <option disabled value="">Please select one</option>
        <option v-for="option in options" :key="option.id" v-bind:value="option.name">
          {{ option.name }}
        </option> -->
    </v-select>
    <!-- <span>Selected: {{ selected }}</span> -->
    <input class="button is-primary margin-bottom margin-top" type="submit" @click.prevent="onChange" value="Get Stats"/>
    <label class="label">Total Streams: {{this.totalStreams}}</label>
    <template>
      <div class="frameworks">
        <framework
        v-for="(dataset, i) in datasets"
        :key="i"
        :data="dataset.data"
        :slug="dataset.slug"
        />
      </div>
    </template>
  </div>
</template>

<script>
import Framework from "./Framework.vue";
import "vue-select/dist/vue-select.css";

export default {
  components: { Framework },
  data() {
    return {
      data: null,
      selected: '',
      options: [],
      accountId:'',
      totalStreams: 0,
      selectMonth: 0,
      selectYear: 0,
      accounts: [],
      months: [
        {id: 0, text: 'Please select month'},
        {id: 1, text: '1'},
        {id: 2, text: '2'},
        {id: 3, text: '3'},
        {id: 4, text: '4'},
        {id: 5, text: '5'},
        {id: 6, text: '6'},
        {id: 7, text: '7'},
        {id: 8, text: '8'},
        {id: 9, text: '9'},
        {id: 10, text: '10'},
        {id: 11, text: '11'},
        {id: 12, text: '12'},
      ],
      years: [
        {id: 0, text: 'Please select year'},
        {id: 2020, text: '2020'},
        {id: 2021, text: '2021'},
        {id: 2022, text: '2022'},
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
    onAccountChange() {
      console.log(this.accountId);
      this.$http
        .get(`http://139.180.139.12/api/artist/account/` + this.accountId)
        .then((res) => {
          this.options = res.data;
          this.selected = '';
        })
        .catch((error) => console.log(error));
    },
    onChange() {
      console.log(this.selected);
      if(this.selected == '') return;
      this.$http
        .get(`http://139.180.139.12/api/accountstream/MonthAndYear?name=` + this.selected + "&month=" + this.selectMonth + "&year=" + this.selectYear)
        .then((res) => {
          this.data = res.data;
          if(res.data[0]){
            this.totalStreams = res.data[0].totalStreams;
          }
        })
        .catch((error) => console.log(error));
    },
    fetchData() {
      this.$http
        .get(`http://139.180.139.12/api/account/getall`)
        .then((res) => {
          this.accounts = res.data;
        })
        .catch((error) => console.log(error));
    },
  },
  mounted() {
    this.fetchData();
  },
};
</script>

<style lang="scss">
.margin-top {
  margin-top: 15px;
}

.margin-bottom {
  margin-bottom: 15px;
}
.margin-left {
  margin-left: 5px;
}

.single-artist .frameworks {
  display: flex;
  flex-wrap: wrap;
  .vtc {
    width: 80%;
    height: 200px;
    margin-right: -5px;
  }

  .stroke {
    stroke-width: 2;
  }

  .fill {
    opacity: 0.2;
  }

  .active-line {
    stroke: rgba(0, 0, 0, 0.2);
  }

  .point {
    display: none;
  }

  .point.is-active {
    display: block;
  }
}

.single-artist .framework {
  margin-top: 20px;
  width: 100%;

  @media (max-width: 699px) {
    &:nth-child(n + 2) {
      margin-top: 30px;
    }

    &:nth-child(n + 4) {
      display: none;
    }
  }

  @media (min-width: 700px) {
    // width: calc(50% - 25px);
    &:nth-child(2n) {
      margin-left: 50px;
    }

    &:nth-child(n + 3) {
      margin-top: 50px;
    }
  }

  &__header {
    display: flex;
    align-items: center;
    justify-content: space-between;
  }

  &__name {
    width: 150px;
  }

  &__period {
    display: flex;
    align-items: center;
    flex-shrink: 0;
    opacity: 0.5;
    &-icon {
      display: block;
      height: 10px;
      width: auto;
      margin-right: 5px;
    }

    &-text {
      font-size: 11px;
      line-height: 16px;
      white-space: nowrap;
    }
  }

  &__data {
    display: flex;
    justify-content: space-between;
    align-items: center;
    width: 100%;
  }

  &__downloads {
    font-size: 24px;
  }

  &.adam {
    border-bottom: 2px solid rgba(#39af77, 0.2);
  }

  &.beck {
    border-bottom: 2px solid rgba(#61dafb, 0.2);
  }

  &.hh {
    border-bottom: 2px solid rgba(#d8002b, 0.2);
  }

  &.v {
    border-bottom: 2px solid rgba(#febc6b, 0.2);
  }

  &.express {
    border-bottom: 2px solid rgba(#259dff, 0.2);
  }

  &.koa {
    border-bottom: 2px solid rgba(#33333d, 0.2);
  }
}

.curve-Adam-Pre, .curve-Adam-Free {
  .stroke {
    stroke: #39af77;
  }

  .fill {
    fill: #39af77;
  }

  .point {
    fill: #39af77;
    stroke: #39af77;
  }
}

.curve-Beck-Pre, .curve-Beck-Free{
  .stroke {
    stroke: #61dafb;
  }

  .fill {
    fill: #61dafb;
  }

  .point {
    fill: #61dafb;
    stroke: #61dafb;
  }
}

.curve-HH-Pre, .curve-HH-Free{
  .stroke {
    stroke: #d8002b;
  }

  .fill {
    fill: #d8002b;
  }

  .point {
    fill: #d8002b;
    stroke: #d8002b;
  }
}

.curve-V-Pre, .curve-V-Free{
  .stroke {
    stroke: #febc6b;
  }

  .fill {
    fill: #febc6b;
  }

  .point {
    fill: #febc6b;
    stroke: #febc6b;
  }
}

.curve-express {
  .stroke {
    stroke: #259dff;
  }

  .fill {
    fill: #259dff;
  }

  .point {
    fill: #259dff;
    stroke: #259dff;
  }
}

.curve-koa {
  .stroke {
    stroke: #33333d;
  }

  .fill {
    fill: #33333d;
  }

  .point {
    fill: #33333d;
    stroke: #33333d;
  }
}
</style>
